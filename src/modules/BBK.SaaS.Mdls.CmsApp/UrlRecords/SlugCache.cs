using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Timing;
using BBK.SaaS.Mdls.Cms.Caching;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.UrlRecords
{
	public interface ISlugCache
	{
		//SlugCacheItem Get(int tenantId);

		SlugCacheItem Get(string slugString);

		//SlugCacheItem GetOrNull(int tenantId);

		SlugCacheItem GetOrNull(string slugString);

		//Task<SlugCacheItem> GetAsync(int tenantId);

		Task<SlugCacheItem> GetAsync(string slugString);

		//Task<SlugCacheItem> GetOrNullAsync(int tenantId);

		Task<SlugCacheItem> GetOrNullAsync(string slugString, bool isViewing = false);
	}

	public class SlugCache : ISlugCache//, IEventHandler<EntityChangedEventData<TSlug>>
	{
		/// <summary>
		/// Reference to the current session.
		/// </summary>
		public IAbpSession AbpSession { get; set; }

		private readonly ICacheManager _cacheManager;
		private readonly IRepository<UrlRecord, long> _slugRepository;
		private readonly ICmsRedisCacheManager _cmsRedisCacheManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		public SlugCache(
			ICacheManager cacheManager,
			IRepository<UrlRecord, long> slugRepository,
			ICmsRedisCacheManager cmsRedisCacheManager,
			IUnitOfWorkManager unitOfWorkManager)
		{
			_cacheManager = cacheManager;
			_slugRepository = slugRepository;
			_cmsRedisCacheManager = cmsRedisCacheManager;
			_unitOfWorkManager = unitOfWorkManager;

			AbpSession = NullAbpSession.Instance;
		}

		public SlugCacheItem Get(string slugString)
		{
			throw new NotImplementedException();
		}

		public async Task<SlugCacheItem> GetAsync(string slugString)
		{
			//if (AbpSession.TenantId == null) { return null; }

			var cacheItem = await GetOrNullAsync(slugString);

			//if (cacheItem == null)
			//{
			//	throw new EntityNotFoundException("There is no tenant with given tenancy name: " + slugString);
			//}

			return cacheItem;
		}

		public SlugCacheItem GetOrNull(string slugString)
		{
			throw new NotImplementedException();
		}

		public async Task<SlugCacheItem> GetOrNullAsync(string slugString, bool isViewing = false)
		{
			int tenantId = AbpSession.TenantId ?? 1;

			var slugItem = await _cacheManager
				.GetSlugCache()
				.GetAsync(
					slugString.ToLowerInvariant(), async key =>
					{
						var urlRecord = await GetUrlRecordOrNullAsync(tenantId, slugString);
						if (urlRecord == null) return null;

						// tungtn: 240125
						//var existedKey = ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount")).GetNumberValueAsync($"{urlRecord.EntityName}_{urlRecord.EntityId}_ViewedCount");

						//if (existedKey != null)
						//{
						//	await _cmsRedisCacheManager.GetCache("ViewedCount").RemoveAsync($"{urlRecord.EntityName}_{urlRecord.EntityId}_ViewedCount");

						//	if (urlRecord.ViewedCount > 0)
						//	{
						//		await ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount")).IncrAsync($"{urlRecord.EntityName}_{urlRecord.EntityId}_ViewedCount", urlRecord.ViewedCount, Clock.Now.AddMinutes(90));
						//	}
						//}
						//else
						//{

						//}
						// END tungtn: 240125

						return CreateSlugCacheItem(urlRecord);
					}
				);

			if (isViewing && slugItem != null &&
				(slugItem.EntityName.Equals(nameof(Article)) /*|| slugItem.EntityName.Equals(nameof(Article))*/))
			{
				// tungtn: 240125
				//await ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount")).IncrAsync($"{slugItem.EntityName}_{slugItem.EntityId}_ViewedCount");
				//await IncrUrlRecordViewedCountAsync(slugItem.Id);
				// END tungtn: 240125

				// incre viewedcount number 
				++slugItem.ViewedCount;

				// sync to cache
				await _cacheManager.GetSlugCache().SetAsync(slugItem.Slug, slugItem);
			}

			return slugItem;
		}

		//public void HandleEvent(EntityChangedEventData<TSlug> eventData)
		//{
		//	throw new NotImplementedException();
		//}

		protected virtual SlugCacheItem CreateSlugCacheItem(UrlRecord urlRecord)
		{
			return new SlugCacheItem
			{
				Id = urlRecord.Id,
				Slug = urlRecord.Slug,
				EntityId = urlRecord.EntityId,
				EntityName = urlRecord.EntityName,
				ViewedCount = urlRecord.ViewedCount
			};
		}

		protected virtual async Task<UrlRecord> GetUrlRecordOrNullAsync(int tenantId, string slugStr)
		{
			return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				return await _slugRepository.FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Slug == slugStr);
			});
		}

		protected virtual async Task<bool> IncrUrlRecordViewedCountAsync(long urlRecordId)
		{
			await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				var i = await _slugRepository.GetAsync(urlRecordId);
				i.ViewedCount++;
			});

			return false;
		}
	}

	[Serializable]
	public class SlugCacheItem
	{
		public const string CacheName = "BBKCmsSlugCache";

		public long TenantId { get; set; }

		public long Id { get; set; }

		public string Slug { get; set; }

		public long EntityId { get; set; }

		public string EntityName { get; set; }

		public long ViewedCount { get; set; } = 0;

		public object CustomData { get; set; }
	}
}
