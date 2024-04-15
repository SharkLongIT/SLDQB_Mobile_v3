using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Cms.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.UrlRecords
{
	public interface ISlugAppService : IApplicationService
	{
		Task<SlugCacheItem> GetSlug(int? tenantId, string slug);
	}

	public class SlugAppService : SaaSAppServiceBase, ISlugAppService
	{
		private readonly IRepository<UrlRecord, long> _urlRecRepository;
		//private readonly ISlugCache _slugCache;

		public SlugAppService(IRepository<UrlRecord, long> urlRecRepository/*, ISlugCache slugCache*/)
		{
			_urlRecRepository = urlRecRepository;
			//_slugCache = slugCache;
		}

		public async Task<SlugCacheItem> GetSlug(int? tenantId, string slug)
		{
			return null;
			//if (tenantId is null)
			//{
			//	throw new ArgumentNullException(nameof(tenantId));
			//}

			//await _slugCache.GetOrNullAsync(slug);
			tenantId ??= AbpSession.TenantId;

			var urlRecorded = await _urlRecRepository.FirstOrDefaultAsync(x => x.Slug == slug);
			if (urlRecorded == null)
			{
				return null;
			}

			var slugItem = new SlugCacheItem();
			slugItem.EntityId = urlRecorded.EntityId;
			slugItem.EntityName = urlRecorded.EntityName;

			if (slug == "about-us")
				return slugItem;
			return slugItem;
		}
	}
}
