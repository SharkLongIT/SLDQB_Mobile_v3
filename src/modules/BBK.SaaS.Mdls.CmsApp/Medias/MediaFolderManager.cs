using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.UI;
using Abp.Zero;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Medias
{
    /// <summary>
    /// Performs domain logic for Organization Units.
    /// </summary>
    public class MediaFolderManager : DomainService
    {
        protected IRepository<MediaFolder, long> mediaFolderUnitRepository { get; private set; }

        public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }
        
        public MediaFolderManager(IRepository<MediaFolder, long> geoUnitRepository)
        {
            this.mediaFolderUnitRepository = geoUnitRepository;

            LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
            AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
        }
        
        public virtual async Task CreateAsync(MediaFolder geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = await GetNextChildCodeAsync(geoUnit.ParentId);
                await ValidateMediaFolderAsync(geoUnit);
                await mediaFolderUnitRepository.InsertAsync(geoUnit);

                await uow.CompleteAsync();
            }    
        }
        
        public virtual void Create(MediaFolder geoUnit)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                geoUnit.Code = GetNextChildCode(geoUnit.ParentId);
                ValidateMediaFolder(geoUnit);
                mediaFolderUnitRepository.Insert(geoUnit);
                
                uow.Complete();
            }
        }

        public virtual async Task UpdateAsync(MediaFolder geoUnit)
        {
            await ValidateMediaFolderAsync(geoUnit);
            await mediaFolderUnitRepository.UpdateAsync(geoUnit);
        }

        public virtual void Update(MediaFolder geoUnit)
        {
            ValidateMediaFolder(geoUnit);
            mediaFolderUnitRepository.Update(geoUnit);
        }

        public virtual async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? await GetCodeAsync(parentId.Value) : null;
                return MediaFolder.AppendCode(parentCode, MediaFolder.CreateCode(1));
            }

            return MediaFolder.CalculateNextCode(lastChild.Code);
        }

        public virtual string GetNextChildCode(long? parentId)
        {
            var lastChild = GetLastChildOrNull(parentId);
            if (lastChild == null)
            {
                var parentCode = parentId != null ? GetCode(parentId.Value) : null;
                return MediaFolder.AppendCode(parentCode, MediaFolder.CreateCode(1));
            }

            return MediaFolder.CalculateNextCode(lastChild.Code);
        }

        public virtual async Task<MediaFolder> GetLastChildOrNullAsync(long? parentId)
        {
            var query = mediaFolderUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        public virtual MediaFolder GetLastChildOrNull(long? parentId)
        {
            var query = mediaFolderUnitRepository.GetAll()
                .Where(ou => ou.ParentId == parentId)
                .OrderByDescending(ou => ou.Code);
            return query.FirstOrDefault();
        }

        public virtual async Task<string> GetCodeAsync(long id)
        {
            return (await mediaFolderUnitRepository.GetAsync(id)).Code;
        }

        public virtual string GetCode(long id)
        {
            return (mediaFolderUnitRepository.Get(id)).Code;
        }
        
        public virtual async Task DeleteAsync(long id)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var children = await FindChildrenAsync(id, true);

                foreach (var child in children)
                {
                    await mediaFolderUnitRepository.DeleteAsync(child);
                }

                await mediaFolderUnitRepository.DeleteAsync(id);

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
                    mediaFolderUnitRepository.Delete(child);
                }

                mediaFolderUnitRepository.Delete(id);
                
                uow.Complete();
            }
        }

        public virtual async Task MoveAsync(long id, long? parentId)
        {
            using (var uow = UnitOfWorkManager.Begin())
            {
                var geoUnit = await mediaFolderUnitRepository.GetAsync(id);
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

                await ValidateMediaFolderAsync(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = MediaFolder.AppendCode(geoUnit.Code, MediaFolder.GetRelativeCode(child.Code, oldCode));
                }
                
                await uow.CompleteAsync();
            }
        }

        public virtual void Move(long id, long? parentId)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                var geoUnit = mediaFolderUnitRepository.Get(id);
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

                ValidateMediaFolder(geoUnit);

                //Update Children Codes
                foreach (var child in children)
                {
                    child.Code = MediaFolder.AppendCode(geoUnit.Code, MediaFolder.GetRelativeCode(child.Code, oldCode));
                }
            });
        }

        public async Task<List<MediaFolder>> FindChildrenAsync(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return await mediaFolderUnitRepository.GetAllListAsync(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return await mediaFolderUnitRepository.GetAllListAsync();
            }

            var code = await GetCodeAsync(parentId.Value);

            return await mediaFolderUnitRepository.GetAllListAsync(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        public List<MediaFolder> FindChildren(long? parentId, bool recursive = false)
        {
            if (!recursive)
            {
                return mediaFolderUnitRepository.GetAllList(ou => ou.ParentId == parentId);
            }

            if (!parentId.HasValue)
            {
                return mediaFolderUnitRepository.GetAllList();
            }

            var code = GetCode(parentId.Value);

            return mediaFolderUnitRepository.GetAllList(
                ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
            );
        }

        protected virtual async Task ValidateMediaFolderAsync(MediaFolder geoUnit)
        {
            var siblings = (await FindChildrenAsync(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("MediaFolderDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }

        protected virtual void ValidateMediaFolder(MediaFolder geoUnit)
        {
            var siblings = (FindChildren(geoUnit.ParentId))
                .Where(ou => ou.Id != geoUnit.Id)
                .ToList();

            if (siblings.Any(ou => ou.DisplayName == geoUnit.DisplayName))
            {
                throw new UserFriendlyException(L("MediaFolderDuplicateDisplayNameWarning", geoUnit.DisplayName));
            }
        }
    }
}
