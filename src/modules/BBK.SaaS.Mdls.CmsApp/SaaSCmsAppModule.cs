using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Abp.Threading.BackgroundWorkers;
using Abp.Zero.Configuration;
using BBK.SaaS.Mdls.Cms.Caching;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Mdls.Cms.UrlRecords;

namespace BBK.SaaS.Mdls.Cms
{
	[DependsOn(
		typeof(SaaSCmsAppSharedModule),
		//, typeof(SaaSApplicationSharedModule)
		//, typeof(SaaSCoreModule)
		typeof(AbpRedisCacheModule)
		)]
	public class SaaSCmsAppModule : AbpModule
	{
		public override void PreInitialize()
		{
			IocManager.Register<ICmsRedisCacheManager, CmsRedisCacheManager>();

			//Adding authorization providers
			//Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

			//Adding custom AutoMapper configuration
			Configuration.Modules.AbpAutoMapper().Configurators.Add(CmsAppMapper.CreateMappings);

			//Adding setting providers
            Configuration.Settings.Providers.Add<CmsAppSettingProvider>();
		}

		public override void Initialize()
		{
			IocManager.RegisterAssemblyByConvention(typeof(SaaSCmsAppModule).GetAssembly());

			Configuration.Navigation.Providers.Add<CmsAppNavigationProvider>();

			RegisterSlugCache();
		}

		public override void PostInitialize()
		{
			var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
			workManager.Add(IocManager.Resolve<SyncViewedCountFromCacheBgWorker>());
		}

		private void RegisterSlugCache()
		{
			if (IocManager.IsRegistered<ISlugCache>())
			{
				return;
			}

			//IocManager.Register(typeof(ISlugCache), DependencyLifeStyle.Transient);
			IocManager.RegisterIfNot<ISlugCache, SlugCache>(DependencyLifeStyle.Transient);
		}
	}
}
