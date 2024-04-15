using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.UI;
using Abp.Zero;

namespace BBK.SaaS.Mdls.Category.Geographies
{
    /// <summary>
    /// Performs domain logic for Organization Units.
    /// </summary>
    public class GeoUnitManager : DomainService
    {
        protected IRepository<GeoUnit, long> geoUnitRepository { get; private set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        
        public GeoUnitManager(IRepository<GeoUnit, long> geoUnitRepository)
        {
            this.geoUnitRepository = geoUnitRepository;

            LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        
        public virtual async Task CreateAsync(GeoUnit geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = await GetNextChildCodeAsync(geoUnit.ParentId);
                await ValidateGeoUnitAsync(geoUnit);
                await geoUnitRepository.InsertAsync(geoUnit);

                await uow.CompleteAsync();
            }    
        }
        
        public virtual void Create(GeoUnit geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = GetNextChildCode(geoUnit.ParentId);
                ValidateGeoUnit(geoUnit);
                geoUnitRepository.Insert(geoUnit);
                
                uow.Complete();
            }
        }

        public virtual async Task UpdateAsync(GeoUnit geoUnit)
        {
            await ValidateGeoUnitAsync(geoUnit);
            await geoUnitRepository.UpdateAsync(geoUnit);
        }

        public virtual void Update(GeoUnit geoUnit)
        {
            ValidateGeoUnit(geoUnit);
            geoUnitRepository.Update(geoUnit);
        }

        public virtual async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return GeoUnit.AppendCode(parentCode, GeoUnit.CreateCode(1));
            }

            return GeoUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual string GetNextChildCode(long? parentId)
        {
            var lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? GetCode(parentId.Value) : null;
                return GeoUnit.AppendCode(parentCode, GeoUnit.CreateCode(1));
            }

            return GeoUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<GeoUnit> GetLastChildOrNullAsync(long? parentId)
        {
            var query = geoUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        public virtual GeoUnit GetLastChildOrNull(long? parentId)
        {
            var query = geoUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return query.FirstOrDefault();
        }

        public virtual async Task<string> GetCodeAsync(long id)
        {
            return (await geoUnitRepository.GetAsync(id)).Code;
        }

        public virtual string GetCode(long id)
        {
            return (geoUnitRepository.Get(id)).Code;
        }
        
        public virtual async Task DeleteAsync(long id)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var children = await FindChildrenAsync(id, true);

                foreach (var child in children)
                {
                    await geoUnitRepository.DeleteAsync(child);
                }

                await geoUnitRepository.DeleteAsync(id);

                await uow.CompleteAsync();
            }
        }

        public virtual void Delete(long id)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var children = FindChildren(id, true);

                foreach (var child in children)
                {
                    geoUnitRepository.Delete(child);
                }

                geoUnitRepository.Delete(id);
                
                uow.Complete();
            }
        }

        public virtual async Task MoveAsync(long id, long? parentId)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var geoUnit = await geoUnitRepository.GetAsync(id);
                if (geoUnit.ParentId == parentId)
                {
                    await uow.CompleteAsync();
                    return;
                }

                //Should find children before Code change
                var children = await FindChildrenAsync(id, true);

                //Store old code of OU
                var oldCode = geoUnit.Code;

                //Move OU
                geoUnit.Code = await GetNextChildCodeAsync(parentId);
                geoUnit.ParentId = parentId;

                await ValidateGeoUnitAsync(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = GeoUnit.AppendCode(geoUnit.Code, GeoUnit.GetRelativeCode(child.Code, oldCode));
                }
                
                await uow.CompleteAsync();
            }
        }

        public virtual void Move(long id, long? parentId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                var geoUnit = geoUnitRepository.Get(id);
                if (geoUnit.ParentId == parentId)
                {
                    return;
                }

                //Should find children before Code change
                var children = FindChildren(id, true);

                //Store old code of OU
                var oldCode = geoUnit.Code;

                //Move OU
                geoUnit.Code = GetNextChildCode(parentId);
                geoUnit.ParentId = parentId;

                ValidateGeoUnit(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = GeoUnit.AppendCode(geoUnit.Code, GeoUnit.GetRelativeCode(child.Code, oldCode));
                }
            });
        }

        public async Task<List<GeoUnit>> FindChildrenAsync(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await geoUnitRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return await geoUnitRepository.GetAllListAsync();
            }

            var code = await GetCodeAsync(parentId.Value);

            return await geoUnitRepository.GetAllListAsync(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        public List<GeoUnit> FindChildren(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return geoUnitRepository.GetAllList(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return geoUnitRepository.GetAllList();
            }

            var code = GetCode(parentId.Value);

            return geoUnitRepository.GetAllList(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        protected virtual async Task ValidateGeoUnitAsync(GeoUnit geoUnit)
        {
            var siblings = (await FindChildrenAsync(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("GeoUnitDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }

        protected virtual void ValidateGeoUnit(GeoUnit geoUnit)
        {
            var siblings = (FindChildren(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("GeoUnitDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }
    }
}
