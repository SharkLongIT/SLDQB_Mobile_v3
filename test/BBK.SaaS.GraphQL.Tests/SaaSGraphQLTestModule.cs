using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using BBK.SaaS.Configure;
using BBK.SaaS.Startup;
using BBK.SaaS.Test.Base;

namespace BBK.SaaS.GraphQL.Tests
{
    [DependsOn(
        typeof(SaaSGraphQLModule),
        typeof(SaaSTestBaseModule))]
    public class SaaSGraphQLTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            IServiceCollection services = new ServiceCollection();
            
            services.AddAndConfigureGraphQL();

            WindsorRegistrationHelper.CreateServiceProvider(IocManager.IocContainer, services);
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSGraphQLTestModule).GetAssembly());
        }
    }
}