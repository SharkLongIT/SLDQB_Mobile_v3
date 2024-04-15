using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.UI;
using Abp.Zero;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.UrlRecords
{
	public class SlugManager : DomainService
	{
		//private readonly IRepository<UrlRecord, long> _slugRepository;
		protected IRepository<UrlRecord, long> UrlRecordRepository { get; private set; }
		public IAbpSession AbpSession { get; set; }
		public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

		public const string TenantSlugCacheName = "SysTenantSlugCache";
		protected ICacheManager CacheManager { get; }

		/// <summary>
		/// "^[a-zA-Z][a-zA-Z0-9_-]{1,}$".
		/// </summary>
		public const string SlugCharacterRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";

		public SlugManager(ICacheManager cacheManager, IRepository<UrlRecord, long> urlRecordRepository)
		{
			UrlRecordRepository = urlRecordRepository;

			LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
			AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
			AbpSession = NullAbpSession.Instance;
			CacheManager = cacheManager;

		}

		public virtual async Task<SlugCacheItem> GetOrNullAsync(string slugString, bool isViewing = false)
		{
			//int cacheKey = (tenantId ?? 1);
			int tenantId = AbpSession.TenantId ?? 1;

			var slugItem = CacheManager
				.GetTenantSlugCache()
				.Get(tenantId, () => new Dictionary<string, SlugCacheItem>());

			var cacheItem = slugItem.GetValueOrDefault(slugString);
			if (cacheItem == null)
			{
				var urlRecord = await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
				{
					return await UrlRecordRepository.FirstOrDefaultAsync(t => t.TenantId == tenantId && t.Slug == slugString);
				});

				if (urlRecord == null) return null;

				cacheItem = new SlugCacheItem()
				{
					EntityId = urlRecord.EntityId,
					EntityName = urlRecord.EntityName,
					Id = urlRecord.Id,
					Slug = urlRecord.Slug,
					ViewedCount = urlRecord.ViewedCount
				};

				slugItem.Add(slugString, cacheItem);

				if (isViewing && slugItem != null &&
				(cacheItem.EntityName.Equals(nameof(Article)) /*|| slugItem.EntityName.Equals(nameof(Article))*/))
				{
					// incre viewedcount number 
					++cacheItem.ViewedCount;

				}

				return cacheItem;
			}
			else
			{
				if (isViewing && slugItem != null &&
				(cacheItem.EntityName.Equals(nameof(Article)) /*|| slugItem.EntityName.Equals(nameof(Article))*/))
				{
					// incre viewedcount number 
					++cacheItem.ViewedCount;

				}
			}

			return cacheItem;
		}

		public virtual Dictionary<string, SlugCacheItem> GetSlugDictionary(int? tenantId, bool isViewing = false)
		{
			//int cacheKey = (tenantId ?? 1);
			int cacheKey = tenantId ?? 1;

			var slugItems = CacheManager
				.GetTenantSlugCache()
				.Get(cacheKey, () => new Dictionary<string, SlugCacheItem>());

			return slugItems;
		}

		//public virtual async Task RegisterSlugAsync(UrlRecord urlRecord)
		//{
		//	using (var uow = UnitOfWorkManager.Begin())
		//	{
		//		if (string.IsNullOrWhiteSpace(urlRecord.Slug))
		//		{
		//			throw new UserFriendlyException(L("DuplicateSlugStringWarning"));
		//		}

		//		await UrlRecordRepository.InsertAsync(urlRecord);

		//		await uow.CompleteAsync();
		//	}
		//}

		public virtual async Task RegisterSlugAsync(UrlRecord record)
		{
			await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
			{
				if (string.IsNullOrWhiteSpace(record.Slug))
				{
					throw new UserFriendlyException(L("EmptySlugStringWarning"));
				}

				await ValidateSlugAsync(record.Slug);

				if (record.EntityId <= 0 || string.IsNullOrWhiteSpace(record.EntityName))
				{
					throw new UserFriendlyException(L("InvalidSlugFormat"));
				}

				if (await UrlRecordRepository.CountAsync(t => t.Slug == record.Slug && t.EntityId != record.EntityId && t.EntityName != record.EntityName) > 0)
				{
					throw new UserFriendlyException(string.Format(L("SlugStringIsAlreadyTaken"), record.Slug));
				}

				if (await UrlRecordRepository.CountAsync(t => t.EntityId == record.EntityId && t.EntityName == record.EntityName) > 0)
				{
					var urlRecord = await UrlRecordRepository.FirstOrDefaultAsync(t => t.EntityId == record.EntityId && t.EntityName == record.EntityName);
					urlRecord.Slug = record.Slug;
					await UrlRecordRepository.UpdateAsync(urlRecord);
				}
				else
				{
					await UrlRecordRepository.InsertAsync(record);
				}

			});
		}

		protected virtual Task ValidateSlugAsync(string slugStr)
		{
			if (!Regex.IsMatch(slugStr, SlugCharacterRegex))
			{
				throw new UserFriendlyException(L("InvalidSlugString"));
			}

			return Task.FromResult(0);
		}
	}
}
