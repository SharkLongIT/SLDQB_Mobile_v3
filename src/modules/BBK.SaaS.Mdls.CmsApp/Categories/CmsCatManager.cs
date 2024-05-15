using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.UI;
using Abp.Zero;
using BBK.SaaS.Mdls.Cms.Entities;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Categories
{
	/// <summary>
	/// Performs domain logic for Category Units.
	/// </summary>
	public class CmsCatManager : DomainService
	{
		protected IRepository<CmsCat, long> CmsCatRepository { get; private set; }

		public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

		public CmsCatManager(IRepository<CmsCat, long> organizationUnitRepository)
		{
			CmsCatRepository = organizationUnitRepository;

			LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
			AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
		}

		public virtual async Task CreateAsync(CmsCat contentCategory)
		{
			using (var uow = UnitOfWorkManager.Begin())
			{
				contentCategory.Code = await GetNextChildCodeAsync(contentCategory.ParentId);
				await ValidateCmsCatAsync(contentCategory);
				await CmsCatRepository.InsertAsync(contentCategory);

				await uow.CompleteAsync();
			}
		}

		public virtual async Task<CmsCat> CreateAndGetIdAsync(CmsCat contentCategory)
		{
			using (var uow = UnitOfWorkManager.Begin())
			{
				contentCategory.Code = await GetNextChildCodeAsync(contentCategory.ParentId);
				await ValidateCmsCatAsync(contentCategory);
				await CmsCatRepository.InsertAndGetIdAsync(contentCategory);

				await uow.CompleteAsync();

				return contentCategory;
			}
		}

		public virtual void Create(CmsCat contentCategory)
		{
			using (var uow = UnitOfWorkManager.Begin())
			{
				contentCategory.Code = GetNextChildCode(contentCategory.ParentId);
				ValidateCmsCat(contentCategory);
				CmsCatRepository.Insert(contentCategory);

				uow.Complete();
			}
		}

		public virtual async Task UpdateAsync(CmsCat contentCategory)
		{
			await ValidateCmsCatAsync(contentCategory);
			await CmsCatRepository.UpdateAsync(contentCategory);
		}

		public virtual void Update(CmsCat contentCategory)
		{
			ValidateCmsCat(contentCategory);
			CmsCatRepository.Update(contentCategory);
		}

		public virtual async Task UpdateOrderIndexAsync(long? parentId, IReadOnlyList<long> sortedIds)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var items = await CmsCatRepository.GetAll()
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
				return CmsCat.AppendCode(parentCode, CmsCat.CreateCode(1));
			}

			return CmsCat.CalculateNextCode(lastChild.Code);
		}

		public virtual string GetNextChildCode(long? parentId)
		{
			var lastChild = GetLastChildOrNull(parentId);
			if (lastChild == null)
			{
				var parentCode = parentId != null ? GetCode(parentId.Value) : null;
				return CmsCat.AppendCode(parentCode, CmsCat.CreateCode(1));
			}

			return CmsCat.CalculateNextCode(lastChild.Code);
		}

		public virtual async Task<CmsCat> GetLastChildOrNullAsync(long? parentId)
		{
			var query = CmsCatRepository.GetAll()
				.Where(ou => ou.ParentId == parentId)
				.OrderByDescending(ou => ou.Code);
			return await AsyncQueryableExecuter.FirstOrDefaultAsync(query);
		}

		public virtual CmsCat GetLastChildOrNull(long? parentId)
		{
			var query = CmsCatRepository.GetAll()
				.Where(ou => ou.ParentId == parentId)
				.OrderByDescending(ou => ou.Code);
			return query.FirstOrDefault();
		}

		public virtual async Task<string> GetCodeAsync(long id)
		{
			return (await CmsCatRepository.GetAsync(id)).Code;
		}

		public virtual string GetCode(long id)
		{
			return (CmsCatRepository.Get(id)).Code;
		}

		public virtual async Task DeleteAsync(long id)
		{
			using (var uow = UnitOfWorkManager.Begin())
			{
				var children = await FindChildrenAsync(id, true);

				foreach (var child in children)
				{
					await CmsCatRepository.DeleteAsync(child);
				}

				await CmsCatRepository.DeleteAsync(id);

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
					CmsCatRepository.Delete(child);
				}

				CmsCatRepository.Delete(id);

				uow.Complete();
			}
		}

		public virtual async Task MoveAsync(long id, long? parentId)
		{
			using (var uow = UnitOfWorkManager.Begin())
			{
				var contentCategory = await CmsCatRepository.GetAsync(id);
				if (contentCategory.ParentId == parentId)
				{
					await uow.CompleteAsync();
					return;
				}

				//Should find children before Code change
				var children = await FindChildrenAsync(id, true);

				//Store old code of OU
				var oldCode = contentCategory.Code;

				//Move OU
				contentCategory.Code = await GetNextChildCodeAsync(parentId);
				contentCategory.ParentId = parentId;

				await ValidateCmsCatAsync(contentCategory);

				//Update Children Codes
				foreach (var child in children)
				{
					child.Code = CmsCat.AppendCode(contentCategory.Code, CmsCat.GetRelativeCode(child.Code, oldCode));
				}

				await uow.CompleteAsync();
			}
		}

		public virtual void Move(long id, long? parentId)
		{
			UnitOfWorkManager.WithUnitOfWork(() =>
			{
				var contentCategory = CmsCatRepository.Get(id);
				if (contentCategory.ParentId == parentId)
				{
					return;
				}

				//Should find children before Code change
				var children = FindChildren(id, true);

				//Store old code of OU
				var oldCode = contentCategory.Code;

				//Move OU
				contentCategory.Code = GetNextChildCode(parentId);
				contentCategory.ParentId = parentId;

				ValidateCmsCat(contentCategory);

				//Update Children Codes
				foreach (var child in children)
				{
					child.Code = CmsCat.AppendCode(contentCategory.Code, CmsCat.GetRelativeCode(child.Code, oldCode));
				}
			});
		}

		public async Task<List<CmsCat>> FindChildrenAsync(long? parentId, bool recursive = false)
		{
			if (!recursive)
			{
				return await CmsCatRepository.GetAllListAsync(ou => ou.ParentId == parentId);
			}

			if (!parentId.HasValue)
			{
				return await CmsCatRepository.GetAllListAsync();
			}

			var code = await GetCodeAsync(parentId.Value);

			return await CmsCatRepository.GetAllListAsync(
				ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
			);
		}

		public List<CmsCat> FindChildren(long? parentId, bool recursive = false)
		{
			if (!recursive)
			{
				return CmsCatRepository.GetAllList(ou => ou.ParentId == parentId);
			}

			if (!parentId.HasValue)
			{
				return CmsCatRepository.GetAllList();
			}

			var code = GetCode(parentId.Value);

			return CmsCatRepository.GetAllList(
				ou => ou.Code.StartsWith(code) && ou.Id != parentId.Value
			);
		}

		protected virtual async Task ValidateCmsCatAsync(CmsCat contentCategory)
		{
			var siblings = (await FindChildrenAsync(contentCategory.ParentId))
				.Where(ou => ou.Id != contentCategory.Id)
				.ToList();

			if (siblings.Any(ou => ou.DisplayName == contentCategory.DisplayName))
			{
				throw new UserFriendlyException(L("CmsCatDuplicateDisplayNameWarning", contentCategory.DisplayName));
			}
		}

		protected virtual void ValidateCmsCat(CmsCat contentCategory)
		{
			var siblings = (FindChildren(contentCategory.ParentId))
				.Where(ou => ou.Id != contentCategory.Id)
				.ToList();

			if (siblings.Any(ou => ou.DisplayName == contentCategory.DisplayName))
			{
				throw new UserFriendlyException(L("CmsCatDuplicateDisplayNameWarning", contentCategory.DisplayName));
			}
		}
	}
}
