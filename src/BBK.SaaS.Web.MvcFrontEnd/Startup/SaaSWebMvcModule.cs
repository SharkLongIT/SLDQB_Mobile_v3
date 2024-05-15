using BBK.SaaS;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using BBK.SaaS.Auditing;
using BBK.SaaS.Authorization.Users.Password;
using BBK.SaaS.Configuration;
using BBK.SaaS.EntityFrameworkCore;
using BBK.SaaS.MultiTenancy;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Mdls.Profile;

namespace BBK.SaaS.Web.Startup
{
    [DependsOn(
        typeof(SaaSWebCoreModule)
    )]
    public class SaaSWebMvcModule : AbpModule
    {
        private readonly IConfigurationRoot _appConfiguration;

        public SaaSWebMvcModule(IWebHostEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void PreInitialize()
        {
            Configuration.Modules.AbpWebCommon().MultiTenancy.DomainFormat = _appConfiguration["App:WebSiteRootAddress"] ?? "https://localhost:44302/";
            Configuration.Navigation.Providers.Add<AppNavigationProvider>();
            //Configuration.Navigation.Providers.Add<SaaSProfileAppNavigationProvider>();

            //Configuration.Navigation.Providers.Add<AppAreaNameNavigationProvider>();

            //IocManager.Register<DashboardViewConfiguration>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSWebMvcModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            if (!IocManager.Resolve<IMultiTenancyConfig>().IsEnabled)
            {
                return;
            }


            using (var scope = IocManager.CreateScope())
            {
                if (!scope.Resolve<DatabaseCheckHelper>().Exist(_appConfiguration["ConnectionStrings:Default"]))
                {
                    return;
                }
            }

            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            //workManager.Add(IocManager.Resolve<SubscriptionExpirationCheckWorker>());
            //workManager.Add(IocManager.Resolve<SubscriptionExpireEmailNotifierWorker>());
            //workManager.Add(IocManager.Resolve<SubscriptionPaymentNotCompletedEmailNotifierWorker>());

            var expiredAuditLogDeleterWorker = IocManager.Resolve<ExpiredAuditLogDeleterWorker>();
            if (Configuration.Auditing.IsEnabled && expiredAuditLogDeleterWorker.IsEnabled)
            {
                workManager.Add(expiredAuditLogDeleterWorker);
            }

            workManager.Add(IocManager.Resolve<PasswordExpirationBackgroundWorker>());
        }
    }
}