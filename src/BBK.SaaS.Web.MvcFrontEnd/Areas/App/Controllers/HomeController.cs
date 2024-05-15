using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Authorization;
using BBK.SaaS.Web.Controllers;
using Abp.Runtime.Session;
using BBK.SaaS.Web.Session;
using BBK.SaaS.Mdls.Category.Geographies;
using System.Linq;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
	[Area("App")]
	[AbpMvcAuthorize]
	public class HomeController : SaaSControllerBase
	{
		private readonly IPerRequestSessionCache _sessionCache;
        private readonly IGeoUnitAppService _geoUnitAppService;

        public HomeController(IPerRequestSessionCache sessionCache, IGeoUnitAppService geoUnitAppService)
		{
			_sessionCache = sessionCache;
			_geoUnitAppService = geoUnitAppService;
		}

		public async Task<ActionResult> Index()
		{
            //var List = _geoUnitAppService.GetAll();
            //if (List.Result.Count() == 0)
            //{
            //    await _geoUnitAppService.BuildDemoGeoAsync();
            //}

            if (AbpSession.MultiTenancySide == MultiTenancySides.Host)
			{
				//if (await IsGrantedAsync(AppPermissions.Pages_Administration_Host_Dashboard))
				//{
				//    return RedirectToAction("Index", "HostDashboard");
				//}

				//if (await IsGrantedAsync(AppPermissions.Pages_Tenants))
				//{
				//    return RedirectToAction("Index", "Tenants");
				//}
				return RedirectToAction("Index", "Tenants");

			}
			else
			{
				//if (await IsGrantedAsync(AppPermissions.Pages_Tenant_Dashboard))
				//{
				//    return RedirectToAction("Index", "TenantDashboard");
				//}

				var currentUserInfo = await _sessionCache.GetCurrentLoginInformationsAsync();

				if (currentUserInfo != null)
				{
					if (currentUserInfo.User.UserType == Authorization.Users.UserTypeEnum.Type2)
					{
                        return RedirectToAction("Index", "Home", new {area = ""});

                    }
					if (currentUserInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
					{
						return RedirectToAction("RecruiterInfo", "Recruiters", new { area = "Profile" });

					}
				}
				return RedirectToAction("Index", "Users");
			}

			//Default page if no permission to the pages above
			return RedirectToAction("Index", "Welcome");
		}
	}
}