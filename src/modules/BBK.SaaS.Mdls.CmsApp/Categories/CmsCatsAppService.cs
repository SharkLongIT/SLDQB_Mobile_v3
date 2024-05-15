using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Extensions;
using Abp.UI;
using BBK.SaaS.Authorization;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mdls.Cms.Entities;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Categories
{
	public class CmsCatsAppService : SaaSAppServiceBase, ICmsCatsAppService
	{
		private readonly IRepository<CmsCat, long> _categoryRepository;
		private readonly IRepository<CmsCatArticle, long> _mapCatArtRepo;
		private readonly IRepository<UrlRecord, long> _slugRepository;

		private readonly CmsCatManager CategoryManager;

		public CmsCatsAppService(IRepository<CmsCat, long> categoryRepository
			, IRepository<CmsCatArticle, long> mapCatArtRepo
			, IRepository<UrlRecord, long> slugRepository
			, CmsCatManager catManager)
		{
			_categoryRepository = categoryRepository;
			_mapCatArtRepo = mapCatArtRepo;
			_slugRepository = slugRepository;
			CategoryManager = catManager;

		}

		public async Task<ListResultDto<CmsCatDto>> GetCmsCats()
		{
			var categories = await _categoryRepository.GetAllListAsync();

			return new ListResultDto<CmsCatDto>(
				categories.Select(ou =>
				{
					var categoryDto = ObjectMapper.Map<CmsCatDto>(ou);
					return categoryDto;
				}).ToList());
		}

		public async Task<ListResultDto<CmsCatDto>> GetCmsCatsByLevel(GetCmsCatInput input)
		{
			var unit = UnitOfWorkManager.Current;
			if (unit.GetTenantId() == null)
			{
				unit.SetTenantId(1);
			}
			var query = _categoryRepository.GetAll()
				.Where(x => x.ParentId == input.ParentId);

			return new ListResultDto<CmsCatDto>(
				(await query.ToListAsync()).Select(ou =>
				{
					var categoryDto = ObjectMapper.Map<CmsCatDto>(ou);
					return categoryDto;
				}).ToList());
		}

		public async Task<ListResultDto<NameValueDto>> GetSortableCategoryUnits(long? id)
		{
			if (id.HasValue && id <= 0)
			{
				throw new UserFriendlyException(L("UnknowIdNumber"));
			}

			var catUnits = await _categoryRepository.GetAll()
				 .Where(x => x.ParentId == id)
				 .OrderBy(x => x.OrderIndex)
				 .ToListAsync();

			return new ListResultDto<NameValueDto>(catUnits.Select(u =>
						new NameValueDto(u.DisplayName, u.Id.ToString())).ToList()
					);
		}
		public async Task SaveSortedCategoryUnits(SortCmsCatInput input)
		{
			await CategoryManager.UpdateOrderIndexAsync(input.ParentId, input.SortedIds);
		}


		//[AbpAuthorize(AppPermissions.Pages_Administration_CmsCats_ManageOrganizationTree)]
		public async Task<CmsCatDto> CreateCmsCat(CreateCmsCatInput input)
		{
			// Ensure slug
			if (string.IsNullOrWhiteSpace(input.Slug))
			{
				input.Slug = Utils.GenerateSlug(input.DisplayName, false);
			}
			else
			{
				input.Slug = Utils.GenerateSlug(input.Slug, false);
			}

			var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(article => article.Slug == input.Slug);
			if (foundSlug != null) { throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); }

			var category = new CmsCat(AbpSession.TenantId.Value, input.DisplayName, input.Slug, input.ParentId);

			//// TODO: Put into a manager
			//await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			//{
			//	var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(article => article.Slug == input.Slug);
			//	if (foundSlug != null) { throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); }

			//	await _articleRepository.InsertAsync(article);
			//	await UnitOfWorkManager.Current.SaveChangesAsync();
			//	await _cmsCatArticleRepository.InsertAsync(new CmsCatArticle(article.TenantId, article.Id, (await _categoryRepository.GetAsync(input.CategoryId)).Id));
			//});

			category = await CategoryManager.CreateAndGetIdAsync(category);

			await _slugRepository.InsertAsync(new UrlRecord() { TenantId = AbpSession.TenantId.Value, EntityId = category.Id, EntityName = nameof(CmsCat), Slug = category.Slug });

			await CurrentUnitOfWork.SaveChangesAsync();

			return ObjectMapper.Map<CmsCatDto>(category);
		}

		//[AbpAuthorize(AppPermissions.Pages_Administration_CmsCats_ManageOrganizationTree)]
		public async Task<CmsCatEditDto> UpdateCmsCat(CmsCatEditDto input)
		{
			// Ensure slug
			if (string.IsNullOrWhiteSpace(input.Slug))
			{
				input.Slug = Utils.GenerateSlug(input.DisplayName, false);
			}
			else
			{
				input.Slug = Utils.GenerateSlug(input.Slug, false);
			}

			var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(article => article.Slug == input.Slug);
			if (foundSlug != null) { throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); }

			//var category = new CmsCat(AbpSession.TenantId.Value, input.DisplayName, input.Slug, input.ParentId);

			// TODO: Put into a manager
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				var updatingItem = await _categoryRepository.GetAsync(input.Id);

				updatingItem.DisplayName = input.DisplayName;
				updatingItem.Slug = input.Slug;
				updatingItem.MetaTitle = input.MetaTitle;
				updatingItem.MetaDescription = input.MetaDescription;
				updatingItem.MetaKeywords = input.MetaKeywords;

				var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(s => s.Slug == updatingItem.Slug && s.EntityId == updatingItem.Id && s.EntityName == nameof(CmsCat));
				if (foundSlug != null)
				{
					foundSlug.Slug = input.Slug;
				}
				else
				{
					_slugRepository.Insert(new UrlRecord() { TenantId = updatingItem.TenantId, EntityId = updatingItem.Id, EntityName = nameof(CmsCat), Slug = input.Slug });
				}
				updatingItem.Slug = input.Slug;

				await _categoryRepository.UpdateAsync(updatingItem);
				await UnitOfWorkManager.Current.SaveChangesAsync();
			});

			return input;
		}

		////[AbpAuthorize(AppPermissions.Pages_Administration_CmsCats_ManageOrganizationTree)]
		public async Task<CmsCatDto> MoveCmsCat(MoveCmsCatInput input)
		{
			await CategoryManager.MoveAsync(input.Id, input.NewParentId);

			return await CreateCmsCatDto(
					await _categoryRepository.GetAsync(input.Id)
			);
		}

		[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs)]
		public async Task DeleteCmsCat(EntityDto<long> input)
		{
			// validate: check any news is in this category or not
			//await _userCmsCatRepository.DeleteAsync(x => x.CmsCatId == input.Id);

			var c = await _mapCatArtRepo.LongCountAsync(x => x.CmsCatId == input.Id);
			if (c > 0)
			{
				throw new UserFriendlyException("ArticlesExisted");
			}

			await CategoryManager.DeleteAsync(input.Id);
		}

		public async Task<List<CmsCatDto>> GetAll()
		{
			var categories = await _categoryRepository.GetAllListAsync();
			return ObjectMapper.Map<List<CmsCatDto>>(categories);
		}

		private async Task<CmsCatDto> CreateCmsCatDto(CmsCat category)
		{
			var dto = ObjectMapper.Map<CmsCatDto>(category);
			//dto.MemberCount =
			//    await _userCmsCatRepository.CountAsync(uou => uou.CmsCatId == category.Id);
			return dto;
		}

		#region Front-end API

		#endregion
	}
}
