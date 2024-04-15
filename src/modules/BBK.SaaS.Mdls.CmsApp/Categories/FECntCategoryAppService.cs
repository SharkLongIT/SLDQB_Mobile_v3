using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using Abp.Linq.Extensions;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Categories;

public class FECntCategoryAppService(IRepository<CmsCat, long> _categoryRepository,
	IRepository<CmsCatArticle, long> _mapCntCatArtRepository,
	IRepository<Article, long> _articleRepository) : SaaSAppServiceBase, IFECntCategoryAppService
{
	public async Task<ListResultDto<ContentCategoryDto>> GetCntCategoriesWithArticles()
	{
		using var uow = UnitOfWorkManager.Begin();
		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			try
			{
				var categories = await _categoryRepository.GetAllListAsync();

				var contentCats = new List<ContentCategoryDto>(
					categories.Select(ou =>
					{
						var categoryDto = ObjectMapper.Map<ContentCategoryDto>(ou);
						return categoryDto;
					}).ToList());

				foreach (var category in contentCats)
				{
					GetArticlesByCatInput input = new() { CategoryId = category.Id, MaxResultCount = 10 };

					var queryCatArticle =
						from catArt in _mapCntCatArtRepository.GetAll()
						where catArt.CmsCatId == input.CategoryId
						select (new { catArt.ArticleId });

					var queryArticles =
						from article in _articleRepository.GetAll()
						join catArt in queryCatArticle on article.Id equals catArt.ArticleId
						orderby article.Id descending
						select article;

					var articles = await queryArticles.PageBy(input).ToListAsync();

					category.Articles = new List<ArticleListViewDto>(articles.Select(article =>
					{
						var viewDto = new ArticleListViewDto(article);

						if (!string.IsNullOrWhiteSpace(viewDto.PrimaryImageUrl))
						{
							viewDto.PrimaryImageUrl = new FileMgr(viewDto.PrimaryImageUrl).FileUrl;
						}

						return viewDto;
					}).ToList());
				}

				return new ListResultDto<ContentCategoryDto>(contentCats);
			}
			finally { await uow.CompleteAsync(); }
		}
	}

	public async Task<ContentCategoryDto> GetCategory(GetCategoryInput input)
	{
		using var uow = UnitOfWorkManager.Begin();
		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			try
			{
				ContentCategoryDto dto = new();
				var category = _categoryRepository.Get(input.CategoryId);

				dto = (ContentCategoryDto)category;

				var query = from catArt in _mapCntCatArtRepository.GetAll()
							//.AsNoTracking()
							join art in _articleRepository.GetAll()/*.AsNoTracking()*/ on catArt.ArticleId equals art.Id
							where catArt.CmsCatId == input.CategoryId
							orderby art.CreationTime descending
							select new
							{
								catArt.CmsCatId,
								article = new ArticleListViewDto()
								{
									Id = art.Id,
									PrimaryImageUrl = art.PrimaryImageUrl,
									//PrimaryImageUrl = (!string.IsNullOrWhiteSpace(art.PrimaryImageUrl) 
									//	? HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(art.PrimaryImageUrl)) 
									//	: string.Empty),
									Slug = art.Slug,
									Title = art.Title,
									ShortDesc = art.ShortDesc,
								}
							};

				var articleCount = query.Count();
				dto.UsedCount = articleCount;

				if (input.SearchArticlesInput.MaxResultCount <= 0) { input.SearchArticlesInput.MaxResultCount = 25; }

				var articles = await query.PageBy(input.SearchArticlesInput).ToListAsync();

				foreach (var c in articles)
				{
					#region Encrypt file url
					c.article.PrimaryImageUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(c.article.PrimaryImageUrl));
					
					#endregion

					dto.Articles.Add(c.article);
				}

				return dto;
			}
			finally { await uow.CompleteAsync(); }
		}
	}

	public async Task<ListResultDto<CmsCatDto>> GetMenuCategories()
	{
		using var uow = UnitOfWorkManager.Begin();
		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			try
			{
				var categories = await _categoryRepository.GetAllListAsync();
				//var categories = await _categoryRepository.GetAll()
				//    .AsNoTracking()
				//    .Where(x => x.ParentId == null).ToListAsync();

				return new ListResultDto<CmsCatDto>(
					categories.Select(ou =>
					{
						var categoryDto = ObjectMapper.Map<CmsCatDto>(ou);
						return categoryDto;
					}).ToList());
			}
			finally { await uow.CompleteAsync(); }
		}
	}
}
