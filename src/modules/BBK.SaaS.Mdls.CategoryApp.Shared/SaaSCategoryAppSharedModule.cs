using Abp.Modules;
using Abp.Reflection.Extensions;
namespace BBK.SaaS.Mdls.Category
{
	public class SaaSCategoryAppSharedModule: AbpModule
	{
		public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(SaaSCategoryAppSharedModule).GetAssembly());
        }
	}
}
