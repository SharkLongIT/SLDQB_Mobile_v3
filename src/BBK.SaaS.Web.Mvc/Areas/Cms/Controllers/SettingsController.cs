using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using BBK.SaaS.Authorization;
using BBK.SaaS.Configuration;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Web.Areas.Cms.Models.Settings;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Areas.Cms.Controllers
{
	[Area("Cms")]
	[AbpMvcAuthorize(AppPermissions.Pages_Administration_CommFuncs)]
	public class SettingsController : SaaSControllerBase
	{
		private readonly IAppConfigurationAccessor _configurationAccessor;
        private readonly ICmsSettingsAppService _cmsSettingsAppService;

		public SettingsController(
            ICmsSettingsAppService cmsSettingsAppService,
			IAppConfigurationAccessor configurationAccessor)
		{
            _cmsSettingsAppService = cmsSettingsAppService;
			_configurationAccessor = configurationAccessor;
		}

		public async Task<ActionResult> Index()
        {
            var output = await _cmsSettingsAppService.GetAllSettings();
            //ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

            //var timezoneItems = await _timingAppService.GetTimezoneComboboxItems(new GetTimezoneComboboxItemsInput
            //{
            //    DefaultTimezoneScope = SettingScopes.Tenant,
            //    SelectedTimezoneId = await SettingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, AbpSession.GetTenantId())
            //});

            //var user = await _userManager.GetUserAsync(AbpSession.ToUserIdentifier());

            //ViewBag.CurrentUserEmail = user.EmailAddress;

            //var tenant = await _tenantManager.FindByIdAsync(AbpSession.GetTenantId());
            //ViewBag.TenantId = tenant.Id;
            //ViewBag.TenantDarkLogoId = tenant.DarkLogoId;
            //ViewBag.TenantLightLogoId = tenant.LightLogoId;
            //ViewBag.TenantCustomCssId = tenant.CustomCssId;

            var model = new CmsSettingsViewModel
            {
                Settings = output,
                //TimezoneItems = timezoneItems
                
            };

            //AddEnabledSocialLogins(model);
            
            return View(model);
        }
	}
}
