using Abp.AutoMapper;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using BBK.SaaS.Authorization;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Configuration;
using BBK.SaaS.Mdls.Profile.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BBK.SaaS.Mdls.Profile
{
	[DependsOn(
		typeof(SaaSCoreModule),
		typeof(SaaSProfileAppSharedModule),
		typeof(SaaSProfileCoreModule)
		)]
	public class SaaSProfileAppModule : AbpModule
	{
		private readonly IConfigurationRoot _appConfiguration;

		public SaaSProfileAppModule(IWebHostEnvironment env)
		{
			_appConfiguration = env.GetAppConfiguration();
		}

		public override void PreInitialize()
		{
			//Adding authorization providers
			Configuration.Authorization.Providers.Add<ProfileAuthorizationProvider>();

			//Adding custom AutoMapper configuration
			Configuration.Modules.AbpAutoMapper().Configurators.Add(SaaSProfileAppMapper.CreateMappings);
		}

		public override void Initialize()
		{
			IocManager.RegisterAssemblyByConvention(typeof(SaaSProfileAppModule).GetAssembly());

			Configuration.Navigation.Providers.Add<SaaSProfileAppNavigationProvider>();

			IocManager.RegisterIfNot<IUserTypeAppService, UserTypeAppService>();
		}

		public override void PostInitialize()
		{

		}
	}
}