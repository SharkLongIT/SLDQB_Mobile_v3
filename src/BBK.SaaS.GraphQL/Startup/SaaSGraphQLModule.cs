using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace BBK.SaaS.Startup
{
    [DependsOn(typeof(SaaSCoreModule))]
    public class SaaSGraphQLModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSGraphQLModule).GetAssembly());
        }

        public override void PreInitialize()
        {
            base.PreInitialize();

            //Adding custom AutoMapper configuration
            Configuration.Modules.AbpAutoMapper().Configurators.Add(CustomDtoMapper.CreateMappings);
        }
    }
}