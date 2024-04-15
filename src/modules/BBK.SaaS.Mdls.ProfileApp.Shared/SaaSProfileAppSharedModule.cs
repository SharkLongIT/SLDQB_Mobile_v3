using Abp.Modules;
using Abp.Reflection.Extensions;

namespace BBK.SaaS.Mdls.Profile
{
    //[DependsOn(typeof(SaaSProfileAppSharedModule))]

	public class SaaSProfileAppSharedModule: AbpModule
	{
		public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSProfileAppSharedModule).GetAssembly());
        }
	}
}
