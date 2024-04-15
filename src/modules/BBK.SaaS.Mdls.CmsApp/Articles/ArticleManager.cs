using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Localization;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using BBK.SaaS.Mdls.Cms.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace BBK.SaaS.Mdls.Cms.Articles
{
	public class ArticleManager : IDomainService
	{
		public const string ArticlesCacheKey = "ArticlesCacheKey";

		public ILocalizationManager LocalizationManager { get; set; }
		public IAbpSession AbpSession { get; set; }
		protected ICacheManager CacheManager { get; }

		private readonly IRepository<Article, long> _articlesRepository;
		private readonly IRepository<CmsCatArticle, long> _catArticleRepo;
		private readonly IRepository<CmsCat, long> _catRepo;
		//private readonly ITypedCache<int, Dictionary<int, ArticleCacheItem>> _articlesCache;

		private readonly IUnitOfWorkManager UnitOfWorkManager;

		public ArticleManager(ICacheManager cacheManager,
			IUnitOfWorkManager unitOfWorkManager, 
			IRepository<Article, long> articlesRepository, 
			IRepository<CmsCat, long> catRepo, 
			IRepository<CmsCatArticle, long> catArticleRepo)
		{

			CacheManager = cacheManager;
			UnitOfWorkManager = unitOfWorkManager;
			_articlesRepository = articlesRepository;
			_catArticleRepo = catArticleRepo;
			_catRepo = catRepo;
			AbpSession = NullAbpSession.Instance;

		}

		public virtual Task<ArticleCacheItem> GetOrNullAsync(long articleId)
		{
			var cacheKey = articleId + "@" + (GetCurrentTenantId() ?? 1);

			//AbpSession.TenantId
			return CacheManager
				.GetArticleCache()
				.GetAsync(cacheKey,
					async () =>
					{
						var article = await _articlesRepository.FirstOrDefaultAsync(articleId);

						//var articleWithCategory = 
						//var query = from a in _articlesRepository.GetAll()
						//			join catArt in _catArticleRepo.GetAll() on a.Id equals catArt.ArticleId
						//			where a.Id == articleId
						//			select new { a, catArt };

						var query = from a in _catArticleRepo.GetAll()
									join cat in _catRepo.GetAll() on a.CmsCatId equals cat.Id
									join catArt in _catArticleRepo.GetAll() on a.Id equals catArt.ArticleId
									where a.Id == articleId
									select new { a, cats = new List<CmsCat>() { new(cat.TenantId) { Id = cat.Id } } };

						var items = await query.ToListAsync();

						return (ArticleCacheItem)article;
					}
				);
		}

		public virtual Task<ArticleCacheItem> GetAsync(long articleId)
		{
			var cacheKey = articleId + "@" + (GetCurrentTenantId() ?? 1);

			return CacheManager
				.GetArticleCache()
				.GetAsync(cacheKey,
					async () =>
					{
						var article = await _articlesRepository.GetAsync(articleId);

						//var query = from a in _catArticleRepo.GetAll()
						//			join cat in _catRepo.GetAll() on a.CmsCatId equals cat.Id
						//			join catArt in _catArticleRepo.GetAll() on a.Id equals catArt.ArticleId
						//			where a.Id == articleId
						//			select new { a, cat };

						//var items = await query.AsNoTracking().ToListAsync();

						////ArticleCacheItem

						//foreach ( var item in items )
						//{
							
						//}

						return (ArticleCacheItem)article;
					}
				);
		}

		private int? GetCurrentTenantId()
		{
			if (UnitOfWorkManager.Current != null)
			{
				return UnitOfWorkManager.Current.GetTenantId();
			}

			return AbpSession.TenantId;
		}
	}

	[Serializable]
	public class ArticleCacheItem
	{
		public long? TenantId { get; set; }

		public long Id { get; set; }

		public string Title { get; set; }

		public string Slug { get; set; }

		public long ViewedCount { get; set; } = 0;

		public string ShortDesc { get; set; }

		public string Content { get; set; }

		public string PrimaryImageUrl { get; set; }

		public string MetaKeywords { get; set; }

		public string MetaDescription { get; set; }

		public string OgTitle { get; set; }

		public string OgDescription { get; set; }

		public string OgImageUrl { get; set; }

		public DateTime Modified { get; set; }

		public string Author { get; set; }

		public static implicit operator ArticleCacheItem(Article a)
		{
			return a == null ? null : new ArticleCacheItem
			{
				Id = a.Id,
				Slug = a.Slug,
				TenantId = a.TenantId,
				Title = a.Title,
				ViewedCount = a.ViewedCount,
				ShortDesc = a.ShortDesc,
				Content = a.Content,
				PrimaryImageUrl = a.PrimaryImageUrl,
				MetaKeywords = a.MetaKeywords,
				MetaDescription = a.MetaDescription,
				OgTitle = a.OgTitle,
				OgDescription = a.OgDescription,
				OgImageUrl = a.OgImageUrl,
				Author = a.Author,
				Modified = a.LastModificationTime ?? a.CreationTime
			};
		}
	}
}
