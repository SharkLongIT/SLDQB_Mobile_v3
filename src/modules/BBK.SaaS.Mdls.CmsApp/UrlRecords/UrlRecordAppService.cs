using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Threading;
using Abp.UI;
using BBK.SaaS.Mdls.Cms.Caching;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.UrlRecords.Dto;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.Mdls.Cms.UrlRecords
{
	public interface IUrlRecordAppService : IApplicationService
	{
		//Task<bool> GetSlug(string slug);
		int SyncViewedCountFromCache();
	}

	public class UrlRecordAppService : SaaSAppServiceBase, IUrlRecordAppService
	{
		private readonly IRepository<UrlRecord, long> _urlRecRepository;
		private readonly ICmsRedisCacheManager _cmsRedisCacheManager;
		private readonly ISlugCache _slugCache;
		private readonly IUnitOfWorkManager _unitOfWorkManager;

		public UrlRecordAppService(IRepository<UrlRecord, long> urlRecRepository, ICmsRedisCacheManager cmsRedisCacheManager, ISlugCache slugCache, IUnitOfWorkManager unitOfWorkManager)
		{
			_urlRecRepository = urlRecRepository;
			_cmsRedisCacheManager = cmsRedisCacheManager;
			_slugCache = slugCache;
			_unitOfWorkManager = unitOfWorkManager;
		}

		public async Task<PagedResultDto<UrlRecordListViewDto>> GetUrlRecordsAsync(GetUrlRecordsInput input)
		{

			var query = _urlRecRepository.GetAll().AsNoTracking()
				.WhereIf(!input.Filter.IsNullOrWhiteSpace(), t => t.Slug.Contains(input.Filter) || t.EntityName.Contains(input.Filter))
				.WhereIf(input.CreationDateStart.HasValue, t => t.CreationTime >= input.CreationDateStart.Value)
				.WhereIf(input.CreationDateEnd.HasValue, t => t.CreationTime <= input.CreationDateEnd.Value);

			var count = await query.CountAsync();
			var topics = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

			return new PagedResultDto<UrlRecordListViewDto>(
				count,
				ObjectMapper.Map<List<UrlRecordListViewDto>>(topics)
				);
		}

		public async Task DeleteUrlRecord(EntityDto<long> entityDto)
		{
			await _urlRecRepository.DeleteAsync(entityDto.Id);
		}


		public int SyncViewedCountFromCache()
		{
			//// tungtn: 240125
			//var allKeys = await ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount")).GetKeys("");
			//List<string> keys = new List<string>();
			//foreach (var key in allKeys)
			//{
			//	if (key.ToString().Contains("Article"))
			//	{
			//		string keyToGet = key.ToString().Replace("n:ViewedCount,c:", "");

			//		var keyValue = await ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount"))
			//			.GetNumberValueAsync(keyToGet);
			//			//.GetOrDefaultAsync(keyToGet);

			//		keys.Add(keyToGet + " has value: " + (keyValue.HasValue ? keyValue.Value : 0));
			//	}
			//}
			//// END tungtn: 240125
			int totalCount = 0;

			_unitOfWorkManager.WithUnitOfWork(() =>
			{
				GetUrlRecordsInput input = new GetUrlRecordsInput() { SkipCount = 0, MaxResultCount = 100, Sorting = "CreationTime DESC" };

				while (true)
				{
					var query = _urlRecRepository.GetAll().Where(c => c.EntityName == nameof(Article));
					//var count = await query.CountAsync();

					var urlRecords = AsyncHelper.RunSync(() => query.OrderBy(input.Sorting).PageBy(input).ToListAsync());

					if (urlRecords == null || urlRecords.Count == 0) { return totalCount; }

					foreach (var urlRecord in urlRecords)
					{
						var cacheItem = AsyncHelper.RunSync(() => _slugCache.GetOrNullAsync(urlRecord.Slug));
						if (cacheItem != null)
						{
							urlRecord.ViewedCount = cacheItem.ViewedCount;
							totalCount++;
						}
					}

					input.SkipCount += 100;
				}

				
			});



			return totalCount;
		}
	}
}
