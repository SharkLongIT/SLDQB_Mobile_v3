using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Areas.App.Models.Layout;
using BBK.SaaS.Web.Views;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Components.AppRecentNotifications
{
    public class AppRecentNotificationsViewComponent : SaaSViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, string iconClass = "flaticon-alert-2 unread-notification fs-2")
        {
            var model = new RecentNotificationsViewModel
            {
                CssClass = cssClass,
                IconClass = iconClass
            };
            
            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }
}
