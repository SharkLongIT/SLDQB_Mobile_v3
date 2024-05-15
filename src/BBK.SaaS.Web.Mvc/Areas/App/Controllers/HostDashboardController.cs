//using Abp.AspNetCore.Mvc.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using BBK.SaaS.Authorization;
//using BBK.SaaS.DashboardCustomization;
//using System.Threading.Tasks;
//using BBK.SaaS.Web.Areas.App.Startup;

//namespace BBK.SaaS.Web.Areas.App.Controllers
//{
//    [Area("App")]
//    [AbpMvcAuthorize(AppPermissions.Pages_Administration_Host_Dashboard)]
//    public class HostDashboardController : CustomizableDashboardControllerBase
//    {
//        public HostDashboardController(
//            DashboardViewConfiguration dashboardViewConfiguration,
//            IDashboardCustomizationAppService dashboardCustomizationAppService)
//            : base(dashboardViewConfiguration, dashboardCustomizationAppService)
//        {

//        }

//        public async Task<ActionResult> Index()
//        {
//            //return await GetView(SaaSDashboardCustomizationConsts.DashboardNames.DefaultHostDashboard);
//        }
//    }
//}