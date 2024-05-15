using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Areas.App.Models.Layout;
using BBK.SaaS.Web.Session;
using BBK.SaaS.Web.Views;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Themes.Default.Components.AppDefaultFooter
{
    public class AppDefaultFooterViewComponent : SaaSViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public AppDefaultFooterViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var footerModel = new FooterViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(footerModel);
        }
    }
}
