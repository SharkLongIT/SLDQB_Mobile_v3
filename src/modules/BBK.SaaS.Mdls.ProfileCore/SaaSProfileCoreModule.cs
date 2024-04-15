using Abp.Modules;
using Abp.Reflection.Extensions;

namespace BBK.SaaS.Mdls.Profile
{
	public class SaaSProfileCoreModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSProfileCoreModule).GetAssembly());
        }
    }
}