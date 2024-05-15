using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Authorization;
using BBK.SaaS.DashboardCustomization;
using System.Threading.Tasks;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Web.Controllers;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
	[Area("App")]
	[AbpMvcAuthorize(AppPermissions.Pages_Tenant_Dashboard)]
	public class TenantDashboardController : SaaSControllerBase//: CustomizableDashboardControllerBase
	{
		//public TenantDashboardController(DashboardViewConfiguration dashboardViewConfiguration, 
		//    IDashboardCustomizationAppService dashboardCustomizationAppService) 
		//    : base(dashboardViewConfiguration, dashboardCustomizationAppService)
		//{

		//}

		public TenantDashboardController() { }

		public async Task<ActionResult> Index()
		{
			//return await GetView(SaaSDashboardCustomizationConsts.DashboardNames.DefaultTenantDashboard);
			return View();
		}
	}
}