using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Areas.App.Models.Layout;
using BBK.SaaS.Web.Session;
using BBK.SaaS.Web.Views;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Themes.Default.Components.AppDefaultBrand
{
    public class AppDefaultBrandViewComponent : SaaSViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public AppDefaultBrandViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var headerModel = new HeaderViewModel
            {
                LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync()
            };

            return View(headerModel);
        }
    }
}
