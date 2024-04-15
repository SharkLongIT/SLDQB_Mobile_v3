using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Threading;
using Abp.Threading.BackgroundWorkers;
using Abp.Threading.Timers;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.UrlRecords
{
	public class SyncViewedCountFromCacheBgWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
	{
		//private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000 * 24; //1 day
		//private const int CheckPeriodAsMilliseconds = 1 * 60 * 60 * 1000; //1 hour
		private const int CheckPeriodAsMilliseconds = 1 * 60 * 45 * 1000; // 45 minutes

		private readonly IRepository<UrlRecord, long> _urlRecRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		//private readonly IPasswordExpirationService _passwordExpirationService;
		private readonly IUrlRecordAppService _urlRecordAppService;
		private readonly SlugManager _slugManager;

		public SyncViewedCountFromCacheBgWorker(
			AbpTimer timer,
			IUnitOfWorkManager unitOfWorkManager
			, IUrlRecordAppService urlRecordAppService
			, SlugManager slugManager
			, IRepository<UrlRecord, long> urlRecRepository
			)
			: base(timer)
		{
			_urlRecRepository = urlRecRepository;
			_unitOfWorkManager = unitOfWorkManager;
			_urlRecordAppService = urlRecordAppService;

			Timer.Period = CheckPeriodAsMilliseconds;
			Timer.RunOnStart = true;

			LocalizationSourceName = SaaSConsts.LocalizationSourceName;
			_slugManager = slugManager;
		}

		protected override void DoWork()
		{
			using (var uow = _unitOfWorkManager.Begin())
			{
				// Switching to host is necessary for single tenant mode.
				using (_unitOfWorkManager.Current.SetTenantId(1))
				{
					//AsyncHelper.RunSync(() => _urlRecordAppService.SyncViewedCountFromCache());
					//_urlRecordAppService.SyncViewedCountFromCache();

					var slugCachedItems = _slugManager.GetSlugDictionary(_unitOfWorkManager.Current.GetTenantId());

					_unitOfWorkManager.WithUnitOfWork(() =>
					{
						foreach (var slug in slugCachedItems)
						{
							if (slug.Value.EntityName == nameof(Article))
							{
								var urlRecord = _urlRecRepository.Get(slug.Value.Id);
								urlRecord.ViewedCount = slug.Value.ViewedCount;
								//_unitOfWorkManager.Current.SaveChanges();
							}
						}			
					});

					uow.Complete();
				}
			}

			//_unitOfWorkManager.WithUnitOfWork(() =>
			//{

			//});

			//List<int> tenantIds;
			//var utcNow = Clock.Now.ToUniversalTime();

			//using (var uow = _unitOfWorkManager.Begin())
			//{
			//	using (_unitOfWorkManager.Current.SetTenantId(null))
			//	{
			//		_userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
			//		tenantIds = _tenantRepository.GetAll().Select(t => t.Id).ToList();
			//		uow.Complete();
			//	}
			//}

			//foreach (var tenantId in tenantIds)
			//{
			//	using (var uow = _unitOfWorkManager.Begin())
			//	{
			//		using (_unitOfWorkManager.Current.SetTenantId(tenantId))
			//		{
			//			_userTokenRepository.Delete(t => t.ExpireDate <= utcNow);
			//			uow.Complete();
			//		}
			//	}
			//}
		}
	}
}
