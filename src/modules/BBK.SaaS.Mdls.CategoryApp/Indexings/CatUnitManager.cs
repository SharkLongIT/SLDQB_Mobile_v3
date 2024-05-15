using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.UI;
using Abp.Zero;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Category.Indexings
{
    /// <summary>
    /// Performs domain logic for Organization Units.
    /// </summary>
    public class CatUnitManager : DomainService
    {
        protected IRepository<CatUnit, long> catUnitRepository { get; private set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        
        public CatUnitManager(IRepository<CatUnit, long> geoUnitRepository)
        {
            this.catUnitRepository = geoUnitRepository;

            LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        
        public virtual async Task CreateAsync(CatUnit geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = await GetNextChildCodeAsync(geoUnit.ParentId);
                await ValidateCatUnitAsync(geoUnit);
                await catUnitRepository.InsertAsync(geoUnit);

                await uow.CompleteAsync();
            }    
        }
        
        public virtual void Create(CatUnit geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = GetNextChildCode(geoUnit.ParentId);
                ValidateCatUnit(geoUnit);
                catUnitRepository.Insert(geoUnit);
                
                uow.Complete();
            }
        }

        public virtual async Task UpdateAsync(CatUnit geoUnit)
        {
            await ValidateCatUnitAsync(geoUnit);
            await catUnitRepository.UpdateAsync(geoUnit);
        }

        public virtual void Update(CatUnit geoUnit)
        {
            ValidateCatUnit(geoUnit);
            catUnitRepository.Update(geoUnit);
        }

    public virtual async Task UpdateOrderIndexAsync(long? parentId, IReadOnlyList<long> sortedIds)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var items = await catUnitRepository.GetAll()
                    .Where(ou => ou.ParentId == parentId).OrderBy(x => x.OrderIndex).ToListAsync();

                for (int i = 0; i < sortedIds.Count; i++)
                {
                    var item = items.FirstOrDefault(x => x.Id == sortedIds[i]);
                    if (item != null)
                        item.OrderIndex = i;
                }
            });
        }

        public virtual async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return CatUnit.AppendCode(parentCode, CatUnit.CreateCode(1));
            }

            return CatUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual string GetNextChildCode(long? parentId)
        {
            var lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? GetCode(parentId.Value) : null;
                return CatUnit.AppendCode(parentCode, CatUnit.CreateCode(1));
            }

            return CatUnit.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<CatUnit> GetLastChildOrNullAsync(long? parentId)
        {
            var query = catUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        public virtual CatUnit GetLastChildOrNull(long? parentId)
        {
            var query = catUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return query.FirstOrDefault();
        }

        public virtual async Task<string> GetCodeAsync(long id)
        {
            return (await catUnitRepository.GetAsync(id)).Code;
        }

        public virtual string GetCode(long id)
        {
            return (catUnitRepository.Get(id)).Code;
        }
        
        public virtual async Task DeleteAsync(long id)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var children = await FindChildrenAsync(id, true);

                foreach (var child in children)
                {
                    await catUnitRepository.DeleteAsync(child);
                }

                await catUnitRepository.DeleteAsync(id);

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
                    catUnitRepository.Delete(child);
                }

                catUnitRepository.Delete(id);
                
                uow.Complete();
            }
        }

        public virtual async Task MoveAsync(long id, long? parentId)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var geoUnit = await catUnitRepository.GetAsync(id);
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

                await ValidateCatUnitAsync(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = CatUnit.AppendCode(geoUnit.Code, CatUnit.GetRelativeCode(child.Code, oldCode));
                }
                
                await uow.CompleteAsync();
            }
        }

        public virtual void Move(long id, long? parentId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                var geoUnit = catUnitRepository.Get(id);
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

                ValidateCatUnit(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = CatUnit.AppendCode(geoUnit.Code, CatUnit.GetRelativeCode(child.Code, oldCode));
                }
            });
        }

        public async Task<List<CatUnit>> FindChildrenAsync(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await catUnitRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return await catUnitRepository.GetAllListAsync();
            }

            var code = await GetCodeAsync(parentId.Value);

            return await catUnitRepository.GetAllListAsync(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        public List<CatUnit> FindChildren(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return catUnitRepository.GetAllList(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return catUnitRepository.GetAllList();
            }

            var code = GetCode(parentId.Value);

            return catUnitRepository.GetAllList(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        protected virtual async Task ValidateCatUnitAsync(CatUnit geoUnit)
        {
            var siblings = (await FindChildrenAsync(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("CatUnitDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }

        protected virtual void ValidateCatUnit(CatUnit geoUnit)
        {
            var siblings = (FindChildren(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("CatUnitDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }
    }
}
