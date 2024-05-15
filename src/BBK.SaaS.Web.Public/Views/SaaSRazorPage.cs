using Abp.AspNetCore.Mvc.Views;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace BBK.SaaS.Web.Public.Views
{
    public abstract class SaaSRazorPage<TModel> : AbpRazorPage<TModel>
    {
        [RazorInject]
        public IAbpSession AbpSession { get; set; }

        protected SaaSRazorPage()
        {
            LocalizationSourceName = SaaSConsts.LocalizationSourceName;
        }
    }
}
