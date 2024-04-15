using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace BBK.SaaS.Mdls.Category
{
	/// <summary>
    /// Application layer module of the application.
    /// </summary>
    [DependsOn(

        typeof(SaaSCategoryAppSharedModule)
        //typeof(SaaSApplicationSharedModule),
        //typeof(SaaSCoreModule)
        )]
	public class SaaSCategoryAppModule: AbpModule
    {
        public override void PreInitialize()
        {
            //Adding authorization providers
            //Configuration.Authorization.Providers.Add<AppAuthorizationProvider>();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(SaaSCategoryAppMapper.CreateMappings);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSCategoryAppModule).GetAssembly());
            
            Configuration.Navigation.Providers.Add<SaaSCategoryAppNavigationProvider>();
        }
    }
}