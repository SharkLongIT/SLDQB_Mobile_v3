using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Mobile.MAUI.Core.ApiClient;

namespace BBK.SaaS
{
    [DependsOn(typeof(SaaSClientModule), typeof(AbpAutoMapperModule))]

    public class SaaSMobileMAUIModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.ReplaceService<IApplicationContext, MAUIApplicationContext>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSMobileMAUIModule).GetAssembly());
        }
    }
}