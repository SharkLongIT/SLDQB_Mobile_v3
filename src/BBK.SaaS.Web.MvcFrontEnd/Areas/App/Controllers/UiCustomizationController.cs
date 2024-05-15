using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Authorization;
using BBK.SaaS.Configuration;
using BBK.SaaS.Web.Areas.App.Models.UiCustomization;
using BBK.SaaS.Web.Controllers;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
    [Area("App")]
    [AbpMvcAuthorize]
    public class UiCustomizationController : SaaSControllerBase
    {
        private readonly IUiCustomizationSettingsAppService _uiCustomizationAppService;

        public UiCustomizationController(IUiCustomizationSettingsAppService uiCustomizationAppService)
        {
            _uiCustomizationAppService = uiCustomizationAppService;
        }

        public async Task<ActionResult> Index()
        {
            var model = new UiCustomizationViewModel
            {
                Theme = await SettingManager.GetSettingValueAsync(AppSettings.UiManagement.Theme),
                Settings = await _uiCustomizationAppService.GetUiManagementSettings(),
                HasUiCustomizationPagePermission = await PermissionChecker.IsGrantedAsync(AppPermissions.Pages_Administration_UiCustomization)
            };

            return View(model);
        }

        public async Task<ActionResult> FormPlay()
        {
            var model = new UiCustomizationViewModel
            {
                Theme = await SettingManager.GetSettingValueAsync(AppSettings.UiManagement.Theme),
                Settings = await _uiCustomizationAppService.GetUiManagementSettings(),
                HasUiCustomizationPagePermission = await PermissionChecker.IsGrantedAsync(AppPermissions.Pages_Administration_UiCustomization)
            };

            return View("FormPlay", model);
            //return View("FormPlayTWo" , model);
        }

        //[HttpPost]
        //public async Task<ActionResult> FormPlay(Dto paymentMethod)
        //{
        //    var model = new UiCustomizationViewModel
        //    {
        //        Theme = await SettingManager.GetSettingValueAsync(AppSettings.UiManagement.Theme),
        //        Settings = await _uiCustomizationAppService.GetUiManagementSettings(),
        //        HasUiCustomizationPagePermission = await PermissionChecker.IsGrantedAsync(AppPermissions.Pages_Administration_UiCustomization)
        //    };

        //    // ABC => db

        //    return View("FormPlayTwo", model);
        //    //return View("FormPlayTWo" , model);
        //}

        public async Task<PartialViewResult> MethodPaymentModal()
        {
            
            // ABC => db

            return PartialView("PaymentMethodView");
            //return View("FormPlayTWo" , model);
        }
    }
}