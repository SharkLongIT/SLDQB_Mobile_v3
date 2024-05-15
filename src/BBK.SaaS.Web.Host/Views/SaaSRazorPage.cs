using Abp.AspNetCore.Mvc.Views;

namespace BBK.SaaS.Web.Views
{
    public abstract class SaaSRazorPage<TModel> : AbpRazorPage<TModel>
    {
        protected SaaSRazorPage()
        {
            LocalizationSourceName = SaaSConsts.LocalizationSourceName;
        }
    }
}
