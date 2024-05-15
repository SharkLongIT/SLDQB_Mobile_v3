using System;
using System.Collections.Generic;
using System.IO;
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
using Abp.IO.Extensions;
using Abp.Linq.Extensions;
using Abp.Runtime.Security;
using Abp.UI;
using BBK.SaaS.Authorization;
using BBK.SaaS.Dto;
using BBK.SaaS.Graphics;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.UrlRecords;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.Articles;

public class ArticlesAppService : SaaSAppServiceBase, IArticlesAppService
{
	private readonly IRepository<Article, long> _articleRepository;
	private readonly IRepository<UrlRecord, long> _slugRepository;
	private readonly SlugManager SlugManager;
	private readonly IRepository<CmsCat, long> _categoryRepository;
	private readonly IRepository<CmsCatArticle, long> _cmsCatArticleRepository;
	private readonly ISlugCache _slugCache;
	private readonly IImageValidator _imageValidator;
	private readonly FileServiceFactory _fileServiceFactory;

	public ArticlesAppService(IRepository<Article, long> topicRepository,
		IRepository<UrlRecord, long> slugRepository,
		SlugManager slugManager,
		IRepository<CmsCat, long> categoryRepository,
		IRepository<CmsCatArticle, long> cmsCatArticleRepository,
		ISlugCache slugCache,
		FileServiceFactory fileServiceFactory,
		IImageValidator imageValidator)
	{
		_articleRepository = topicRepository;
		_slugRepository = slugRepository;
		SlugManager = slugManager;
		_categoryRepository = categoryRepository;
		_cmsCatArticleRepository = cmsCatArticleRepository;
		_slugCache = slugCache;
		_fileServiceFactory = fileServiceFactory;
		_imageValidator = imageValidator;
	}

	#region Admins API
	public async Task<PagedResultDto<ArticleListDto>> GetArticlesAsync(GetArticlesInput input)
	{
		var query = _articleRepository.GetAll().AsNoTracking()
			.WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Title.Contains(input.Filter) || t.Title.Contains(input.Filter))
			.WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
			.WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);

		var count = await query.CountAsync();
		var topics = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

		return new PagedResultDto<ArticleListDto>(
			count,
			ObjectMapper.Map<List<ArticleListDto>>(topics)
			);
	}

	public async Task<ArticleEditDto> CreateAsync(CreateArticleInput input)
	{
		if (!string.IsNullOrEmpty(input.PrimaryImageUrl))
		{
			input.PrimaryImageUrl = StringCipher.Instance.Decrypt(input.PrimaryImageUrl);
		}



		#region Ensure slug
		if (string.IsNullOrWhiteSpace(input.Slug))
		{
			input.Slug = Utils.GenerateSlug(input.Title, false);
		}
		else
		{
			input.Slug = Utils.GenerateSlug(input.Slug, false);
		}
		#endregion

		Article article = ObjectMapper.Map<Article>(input);
		article.TenantId = AbpSession.TenantId.Value;

		#region Secure Html Inject
		input.Content = CleanHtml(new NameValueDto() { Value = input.Content }).Value;
		#endregion

		// TODO: Put into a manager
		await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
		{
			//var foundSlug = await _slugRepository.GetAll().FirstOrDefaultAsync(article => article.Slug == input.Slug);
			//if (foundSlug != null) { throw new UserFriendlyException("Search engine friendly page name is duplicated!!!"); }

			await _articleRepository.InsertAsync(article);
			await UnitOfWorkManager.Current.SaveChangesAsync();

			await _cmsCatArticleRepository.InsertAsync(new CmsCatArticle(article.TenantId, article.Id, (await _categoryRepository.GetAsync(input.CategoryId)).Id));

			await SlugManager.RegisterSlugAsync(new UrlRecord() { TenantId = AbpSession.TenantId.Value, EntityId = article.Id, EntityName = nameof(Article), Slug = article.Slug });

			// Processing image from browser
			if (!string.IsNullOrEmpty(input.PrimaryImageData) && input.PrimaryImageData.StartsWith("data:image/png"))
			{
				FileInputDto fileInput = new();
				fileInput.FileName = $"articleimg_{AbpSession.TenantId.Value}_{article.Id}.webp";
				fileInput.TenantId = AbpSession.TenantId.Value;
				fileInput.CreatedAt = DateTime.Now;
				fileInput.FileCategory = "CmsArticles";
				fileInput.IsUniqueFileName = false;
				fileInput.IsUniqueFolder = false;

				using (var fileService = _fileServiceFactory.Get())
				{
					byte[] fileContent = Convert.FromBase64String(input.PrimaryImageData.Replace("data:image/png;base64,", ""));
					using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent, 800, 500)))
					{
						//var fileMgr = await fileService.Object.CreateOrUpdateImage(outputStream, fileInput);
						var fileMgr = await fileService.Object.CreateOrUpdate(outputStream.GetAllBytes(), fileInput);
						article.PrimaryImageUrl = fileInput.FilePath;
					}

					// make thumbnail
					fileInput.FileName = $"articleimg_{AbpSession.TenantId.Value}_{article.Id}_thumb.webp";
					_ = fileService.Object.CreateOrUpdate(_imageValidator.MakeThumbnail(fileContent, 240, 150), fileInput);
				}
			}

			//await _slugRepository.InsertAsync(new UrlRecord() { TenantId = AbpSession.TenantId.Value, EntityId = article.Id, EntityName = nameof(Article), Slug = article.Slug });
		});

		ArticleEditDto result = ObjectMapper.Map<ArticleEditDto>(article);

		return result;
	}

	public async Task<ArticleEditDto> UpdateAsync(ArticleEditDto input)
	{
		if (!string.IsNullOrEmpty(input.PrimaryImageUrl))
		{
			input.PrimaryImageUrl = StringCipher.Instance.Decrypt(input.PrimaryImageUrl);
		}

		// Processing image from browser
		if (!string.IsNullOrEmpty(input.PrimaryImageData) && input.PrimaryImageData.StartsWith("data:image/png"))
		{
			FileInputDto fileInput = new();
			fileInput.FileName = $"articleimg_{AbpSession.TenantId.Value}_{input.Id}.webp";
			fileInput.TenantId = AbpSession.TenantId.Value;
			fileInput.CreatedAt = DateTime.Now;
			fileInput.FileCategory = "CmsArticles";
			fileInput.IsUniqueFileName = false;
			fileInput.IsUniqueFolder = false;

			using (var fileService = _fileServiceFactory.Get())
			{
				byte[] fileContent = Convert.FromBase64String(input.PrimaryImageData.Replace("data:image/png;base64,", ""));
				using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent, 800, 500)))
				{
					//var fileMgr = await fileService.Object.CreateOrUpdateImage(outputStream, fileInput);
					var fileMgr = await fileService.Object.CreateOrUpdate(outputStream.GetAllBytes(), fileInput);
					input.PrimaryImageUrl = fileInput.FilePath;
				}

				// make thumbnail
				fileInput.FileName = $"articleimg_{AbpSession.TenantId.Value}_{input.Id}_thumb.webp";
				_ = fileService.Object.CreateOrUpdate(_imageValidator.MakeThumbnail(fileContent, 240, 150), fileInput);
			}
		}

		// Ensure slug
		if (string.IsNullOrWhiteSpace(input.Slug))
		{
			input.Slug = Utils.GenerateSlug(input.Title, false);
		}
		else
		{
			input.Slug = Utils.GenerateSlug(input.Slug, false);
		}

		Article article = await _articleRepository.GetAsync(input.Id);
		article = ObjectMapper.Map(input, article);

		#region Secure Html Inject
		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(input.Content);
		HtmlNodeCollection divs = htmlDoc.DocumentNode.SelectNodes("//*[@style]");
		if (divs != null)
		{
			foreach (HtmlNode div in divs)
			{
				div.Attributes.RemoveAll();
			}
		}
		input.Content = htmlDoc.DocumentNode.OuterHtml;

		HtmlNodeCollection scripts = htmlDoc.DocumentNode.SelectNodes("//script");
		if (scripts != null)
			scripts.ToList().ForEach(n => n.Remove());
		#endregion

		// TODO: Put into a manager
		await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
		{
			// delete exsited category
			await _cmsCatArticleRepository.DeleteAsync(x => x.ArticleId == input.Id);
			await UnitOfWorkManager.Current.SaveChangesAsync();

			if (input.CategoryId.HasValue)
				await _cmsCatArticleRepository.InsertAsync(new CmsCatArticle(article.TenantId, article.Id, (await _categoryRepository.GetAsync(input.CategoryId.Value)).Id));

			await SlugManager.RegisterSlugAsync(new UrlRecord() { TenantId = article.TenantId, EntityId = article.Id, EntityName = nameof(Article), Slug = input.Slug });
			article.Slug = input.Slug;

			await _articleRepository.UpdateAsync(article);
			await UnitOfWorkManager.Current.SaveChangesAsync();
			//await _cmsCatArticleRepository.InsertAsync(new CmsCatArticle(article.TenantId, article.Id, (await _categoryRepository.GetAsync(input.CategoryId)).Id));
		});

		ArticleEditDto result = ObjectMapper.Map<ArticleEditDto>(article);

		return result;
	}

	[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs_Delete)]
	public async Task<bool> DeleteArticleAsync(EntityDto<long> input)
	{
		var entity = await _articleRepository.GetAsync(input.Id);
		await _articleRepository.DeleteAsync(entity);

		return true;
	}

	public async Task<ArticleEditDto> GetArticleForEditAsync(EntityDto<long> entityDto)
	{
		var entity = await _articleRepository.GetAsync(entityDto.Id);

		var dto = ObjectMapper.Map<ArticleEditDto>(entity);

		var categoryId = await _cmsCatArticleRepository.FirstOrDefaultAsync(x => x.ArticleId == entityDto.Id);
		dto.CategoryId = categoryId?.CmsCatId;

		//#region Encrypted FileURL
		//if (!string.IsNullOrEmpty(entity.PrimaryImageUrl))
		//	dto.PrimaryImage = new FileMgr(entity.PrimaryImageUrl);
		//#endregion

		return dto;
	}

	public NameValueDto CleanHtml(NameValueDto input)
	{
		#region Secure Html Inject
		var htmlDoc = new HtmlDocument();
		htmlDoc.LoadHtml(input.Value);
		HtmlNodeCollection divs = htmlDoc.DocumentNode.SelectNodes("//*[@style]");
		if (divs != null)
		{
			foreach (HtmlNode div in divs)
			{
				div.Attributes["style"].Remove();
			}
		}
		input.Value = htmlDoc.DocumentNode.OuterHtml;

		HtmlNodeCollection eles = htmlDoc.DocumentNode.SelectNodes("//*[@width]");
		if (eles != null)
		{
			foreach (HtmlNode ele in eles)
			{
				ele.Attributes["width"].Remove();
			}
		}

		HtmlNodeCollection scripts = htmlDoc.DocumentNode.SelectNodes("//script");
		if (scripts != null)
			scripts.ToList().ForEach(n => n.Remove());
		#endregion

		//input.Value = string.Empty;

		return input;
	}
	#endregion

	#region FrontEnd API
	public async Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input)
	{
		var unit = UnitOfWorkManager.Current;
		if (unit.GetTenantId() == null)
		{
			unit.SetTenantId(1);
		}
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
			PrimaryImageUrl = article.PrimaryImageUrl,
			Slug = article.Slug,
			Author = article.Author,

		}).ToList();
		return new ListResultDto<ArticleListViewDto>(category);
	}

	public async Task<ListResultDto<ArticleListViewDto>> GetFEArticles(SearchArticlesInput input)
	{
		// prepare cache service

		using var uow = UnitOfWorkManager.Begin();
		using (CurrentUnitOfWork.SetTenantId(input.TenantId ?? 1))
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
					PrimaryImageUrl = $"/file/get?c=" + HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(article.PrimaryImageUrl)),//article.PrimaryImageUrl,
					Slug = article.Slug,
					Author = article.Author,
					Modified = article.CreationTime,

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

	public async Task<ListResultDto<ArticleListViewDto>> GetFENewestArticles()
	{
		// prepare cache service

		using var uow = UnitOfWorkManager.Begin();

		using (CurrentUnitOfWork.SetTenantId(AbpSession.TenantId ?? 1))
		{
			try
			{
				var query = _articleRepository.GetAll();
				//.WhereIf(input.CategoryId.HasValue, a => _cmsCatArticleRepository.GetAll().Where(x => x.CmsCatId == input.CategoryId).Select(x => x.CmsCatId).Contains(a.Id));

				query = query.OrderByDescending(x => x.CreationTime).PageBy(new PagedAndSortedInputDto() { MaxResultCount = 8 });

				var listArticles = await query.ToListAsync();
				var category = listArticles.Select(article => new ArticleListViewDto
				{

					Id = article.Id,
					Title = article.Title,
					ShortDesc = article.ShortDesc,
					PrimaryImageUrl = $"/file/get?c=" + HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(article.PrimaryImageUrl)),
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

	public async Task<ArticleViewDto> GetFEArticleDetail(EntityDto<long> entityDto)
	{
		var unit = UnitOfWorkManager.Current;
		if (unit.GetTenantId() == null)
		{
			unit.SetTenantId(1);
		}
		var article = await _articleRepository.GetAsync(entityDto.Id);


		var dto = ObjectMapper.Map<ArticleViewDto>(article);

		var viewedCount = await _slugCache.GetOrNullAsync(article.Slug);
		if (viewedCount != null)
		{
			dto.ViewedCount = viewedCount.ViewedCount;
		}

		#region Encrypted FileURL
		if (!string.IsNullOrEmpty(article.PrimaryImageUrl))
		{
			dto.PrimaryImage = new FileMgr(article.PrimaryImageUrl);
			dto.PrimaryImageUrl = dto.PrimaryImage.FileUrl;
		}
		#endregion

		return dto;
	}
	#endregion

}

