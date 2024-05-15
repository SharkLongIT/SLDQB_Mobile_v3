using Abp.AspNetCore.Mvc.ViewComponents;

namespace BBK.SaaS.Web.Public.Views
{
    public abstract class SaaSViewComponent : AbpViewComponent
    {
        protected SaaSViewComponent()
        {
            LocalizationSourceName = SaaSConsts.LocalizationSourceName;
        }
    }
}