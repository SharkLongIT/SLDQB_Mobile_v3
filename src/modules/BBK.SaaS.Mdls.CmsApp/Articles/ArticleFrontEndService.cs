using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Web;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Abp.UI;
using BBK.SaaS.Authorization;
using BBK.SaaS.Dto;
using BBK.SaaS.Graphics;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.UrlRecords;
using BBK.SaaS.Net;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Articles;

public class ArticleFrontEndService : SaaSAppServiceBase, IArticleFrontEndService
{
	private readonly IRepository<Article, long> _articleRepository;
	private readonly ArticleManager ArticleManager;
	private readonly IRepository<UrlRecord, long> _slugRepository;
	private readonly SlugManager SlugManager;
	private readonly IRepository<CmsCat, long> _categoryRepository;
	private readonly IRepository<CmsCatArticle, long> _cmsCatArticleRepository;
	private readonly ISlugCache _slugCache;
	private readonly FileServiceFactory _fileServiceFactory;


	public ArticleFrontEndService(IRepository<Article, long> topicRepository,
		ArticleManager ArticleManager,
		IRepository<UrlRecord, long> slugRepository,
		SlugManager slugManager,
		IRepository<CmsCat, long> categoryRepository,
		IRepository<CmsCatArticle, long> cmsCatArticleRepository,
		FileServiceFactory fileServiceFactory,
		ISlugCache slugCache)
	{
		_articleRepository = topicRepository;
		this.ArticleManager = ArticleManager;
		_slugRepository = slugRepository;
		SlugManager = slugManager;
		_categoryRepository = categoryRepository;
		_cmsCatArticleRepository = cmsCatArticleRepository;
		_fileServiceFactory = fileServiceFactory;
		_slugCache = slugCache;

	}

	#region FrontEnd API
	public async Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input)
	{
		using var uow = UnitOfWorkManager.Begin();

		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			try
			{
				var queryCatArticle =
					from catArt in _cmsCatArticleRepository.GetAll()
					where catArt.CmsCatId == input.CategoryId
					select (new { catArt.ArticleId });

				var queryArticles =
					from article in _articleRepository.GetAll()
					join catArt in queryCatArticle on article.Id equals catArt.ArticleId
					orderby article.Id descending
					select article;

				var articles = await queryArticles.PageBy(input).ToListAsync();

				var category = articles.Select(article => new ArticleListViewDto
				{
					Id = article.Id,
					Title = article.Title,
					ShortDesc = article.ShortDesc,
					//PrimaryImageUrl = article.PrimaryImageUrl,
					PrimaryImageUrl = string.IsNullOrWhiteSpace(article.PrimaryImageUrl) ? string.Empty : (new FileMgr(article.PrimaryImageUrl)).FileUrl,
					Slug = article.Slug,
					Author = article.Author,

				}).ToList();

				return new ListResultDto<ArticleListViewDto>(category);
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				await uow.CompleteAsync();
			}
		}
	}

	public async Task<ListResultDto<ArticleListViewDto>> GetArticles(SearchArticlesInput input)
	{
		// prepare cache service

		// begin UnitOfWork for ViewComponents or Mobile
		using var uow = UnitOfWorkManager.Begin();

		using (CurrentUnitOfWork.SetTenantId(input.TenantId ?? AbpSession.TenantId ?? 1))
		{
			try
			{
				var query = _articleRepository.GetAll()
					.WhereIf(input.CategoryId.HasValue, a => _cmsCatArticleRepository.GetAll().Where(x => x.CmsCatId == input.CategoryId).Select(x => x.CmsCatId).Contains(a.Id));

				query = query.OrderByDescending(x => x.CreationTime).PageBy(input);

				var listArticles = await query.ToListAsync();
				//"/file/get?c=" + HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(filePath))
				var category = listArticles.Select(article => new ArticleListViewDto
				{
					Id = article.Id,
					Title = article.Title,
					ShortDesc = article.ShortDesc,
					//PrimaryImageUrl = !string.IsNullOrEmpty(article.PrimaryImageUrl) ? HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(article.PrimaryImageUrl)) : string.Empty, //article.PrimaryImageUrl,
					//PrimaryImageUrl = !string.IsNullOrEmpty(article.PrimaryImageUrl) ? HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(ImageHelper.GetThumbnailImage(article.PrimaryImageUrl))) : string.Empty, //article.PrimaryImageUrl,
					PrimaryImageUrl = !string.IsNullOrEmpty(article.PrimaryImageUrl) ? HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(article.PrimaryImageUrl)) : string.Empty, //article.PrimaryImageUrl,

					Slug = article.Slug,
					Author = article.Author,

				}).ToList();

				return new ListResultDto<ArticleListViewDto>(category);
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				await uow.CompleteAsync();
			}
		}
	}

	public async Task<ListResultDto<ArticleListViewDto>> GetNewestArticles()
	{
		// prepare cache service

		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			var query = _articleRepository.GetAll();
			//.WhereIf(input.CategoryId.HasValue, a => _cmsCatArticleRepository.GetAll().Where(x => x.CmsCatId == input.CategoryId).Select(x => x.CmsCatId).Contains(a.Id));

			query = query.OrderByDescending(x => x.CreationTime).PageBy(new PagedAndSortedInputDto() { MaxResultCount = 25 });

			var listArticles = await query.ToListAsync();
			var category = listArticles.Select(article => new ArticleListViewDto
			{

				Id = article.Id,
				Title = article.Title,
				ShortDesc = article.ShortDesc,
				PrimaryImageUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(article.PrimaryImageUrl)),
				Slug = article.Slug,
				Author = article.Author,

			}).ToList();

			return new ListResultDto<ArticleListViewDto>(category);
		}
	}

	public async Task<ArticleViewDto> GetArticleDetail(EntityDto<long> entityDto)
	{
		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			if (entityDto.Id <= 0) throw new UserFriendlyException("NotFoundArticle");

			var cachedItem = await ArticleManager.GetAsync(entityDto.Id);

			var dto = ObjectMapper.Map<ArticleViewDto>(cachedItem);

			// get relate categories in Article
			//var query = from catArt in _cmsCatArticleRepository.GetAll().AsNoTracking()
			//			join art in _articleRepository.GetAll().AsNoTracking() on catArt.ArticleId equals art.Id
			//			join cat in _categoryRepository.GetAll().AsNoTracking() on catArt.CmsCatId equals cat.Id
			//			where catArt.ArticleId == entityDto.Id
			//			select new { cat, art };

			var catId = await _cmsCatArticleRepository.FirstOrDefaultAsync(x => x.ArticleId == entityDto.Id);
			if (catId != null)
			{
				//if (dto.Categories is null) dto.Categories = new List<Categories.MDto.ContentCategoryDto>();
				dto.Categories = [new Categories.MDto.ContentCategoryDto() { Id = catId.CmsCatId }];
			}

			var slugCache = await SlugManager.GetOrNullAsync(cachedItem.Slug);
			if (slugCache != null)
			{
				dto.ViewedCount = slugCache.ViewedCount;
			}

			#region Encrypted FileURL
			if (!string.IsNullOrEmpty(cachedItem.PrimaryImageUrl))
			{
				dto.PrimaryImage = new FileMgr(cachedItem.PrimaryImageUrl);
				dto.PrimaryImageUrl = dto.PrimaryImage.FileUrl;
			}
			#endregion

			return dto;
		}
	}

	[AbpAllowAnonymous]
	public async Task<string> GetPicture(string encryptedUrl)
	{
		if (!string.IsNullOrEmpty(encryptedUrl))
		{
			if (encryptedUrl.StartsWith("/file/get?c="))
			{
				encryptedUrl = encryptedUrl["/file/get?c=".Length..];
			}

			int tenantId = AbpSession.TenantId ?? 1;
			var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = StringCipher.Instance.Decrypt(encryptedUrl) };

			using var profileImageService = _fileServiceFactory.Get();

			var profileImage = await profileImageService.Object.Download(fileMgr);
			if (profileImage == null) return string.Empty;

			//return $"data:{(!string.IsNullOrEmpty(profileImage.FileType) ? profileImage.FileType : MimeTypeNames.ApplicationOctetStream)};{profileImage.ContentString}";
			return profileImage.ContentString;
		}
		return string.Empty;
	}
	#endregion

}

