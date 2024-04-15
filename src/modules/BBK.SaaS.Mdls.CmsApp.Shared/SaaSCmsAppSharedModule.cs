using Abp.Modules;
using Abp.Reflection.Extensions;
using System;

namespace BBK.SaaS.Mdls.Cms
{
	//[DependsOn(SaaSCoreSharedModule)]
	public class SaaSCmsAppSharedModule : AbpModule
	{
		public override void Initialize()
		{
			IocManager.RegisterAssemblyByConvention(typeof(SaaSCmsAppSharedModule).GetAssembly());
		}
	}
}
