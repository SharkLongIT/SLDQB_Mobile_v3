using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.Domain.Uow;
using Abp.Linq;
using Abp.UI;
using Abp.Zero;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Categories
{
	public class CategoryArticleManager : DomainService
	{
		protected IRepository<CmsCat, long> CmsCatRepository { get; private set; }

		public IAsyncQueryableExecuter AsyncQueryableExecuter { get; set; }

		//private readonly ITypedCache<int, Dictionary<int, CategoryArticleCacheItem>> _articlesCache;

		public CategoryArticleManager(IRepository<CmsCat, long> cmsCatRepository)
		{
			CmsCatRepository = cmsCatRepository;

			LocalizationSourceName = AbpZeroConsts.LocalizationSourceName;
			AsyncQueryableExecuter = NullAsyncQueryableExecuter.Instance;
		}

		
	}

	[Serializable]
	public class CategoryArticleCacheItem
	{
		public long? TenantId { get; set; }

		public long Id { get; set; }

		public string Title { get; set; }

		public string Code { get; set; }

		public string Slug { get; set; }

		public long ViewedCount { get; set; } = 0;

		
	}
}
