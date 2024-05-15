using Abp.Modules;
using Abp.Reflection.Extensions;

namespace BBK.SaaS
{
    public class SaaSClientModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSClientModule).GetAssembly());
        }
    }
}
