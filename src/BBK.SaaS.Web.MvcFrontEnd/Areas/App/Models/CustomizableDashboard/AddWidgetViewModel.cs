using System.Collections.Generic;
using BBK.SaaS.DashboardCustomization.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.CustomizableDashboard
{
    public class AddWidgetViewModel
    {
        public List<WidgetOutput> Widgets { get; set; }

        public string DashboardName { get; set; }

        public string PageId { get; set; }
    }
}
