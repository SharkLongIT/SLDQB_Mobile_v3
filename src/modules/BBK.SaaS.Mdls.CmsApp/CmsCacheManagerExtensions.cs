using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.UrlRecords;

namespace BBK.SaaS.Mdls.Cms
{
	public static class CmsCacheManagerExtensions
	{
		public static ITypedCache<string, SlugCacheItem> GetSlugCache(this ICacheManager cacheManager)
		{
			return cacheManager.GetCache<string, SlugCacheItem>(SlugCacheItem.CacheName);
		}

		public static ITypedCache<int, Dictionary<string, SlugCacheItem>> GetTenantSlugCache(this ICacheManager cacheManager)
		{
			return cacheManager.GetCache<int, Dictionary<string, SlugCacheItem>>(SlugManager.TenantSlugCacheName);
		}

		//public static ITypedCache<int, Dictionary<int, ArticleCacheItem>> GetArticleCache(this ICacheManager cacheManager)
		//{
		//	return cacheManager.GetCache<int, Dictionary<int, ArticleCacheItem>>(ArticleManager.ArticlesCacheKey);
		//}

		public static ITypedCache<string, ArticleCacheItem> GetArticleCache(this ICacheManager cacheManager)
		{
			return cacheManager.GetCache<string, ArticleCacheItem>(ArticleManager.ArticlesCacheKey);
		}
	}
}
