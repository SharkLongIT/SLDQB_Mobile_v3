using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Areas.App.Models.Layout;
using BBK.SaaS.Web.Views;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Components.AppQuickThemeSelect
{
namespace BBK.SaaS.Web.Areas.App.Views.Shared.Components.
    AppQuickThemeSelect
{
    public class AppQuickThemeSelectViewComponent : SaaSViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, string iconClass = "flaticon-interface-7 fs-2")
        {
            return Task.FromResult<IViewComponentResult>(View(new QuickThemeSelectionViewModel
            {
                CssClass = cssClass,
                IconClass = iconClass
            }));
        }
    }
}
}