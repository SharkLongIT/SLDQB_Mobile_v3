using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Abp.Timing;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Authorization;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Configuration;
using BBK.SaaS.Configuration.Tenants;
using BBK.SaaS.MultiTenancy;
using BBK.SaaS.Security;
using BBK.SaaS.Timing;
using BBK.SaaS.Timing.Dto;
using BBK.SaaS.Web.Areas.App.Models.Settings;
using BBK.SaaS.Web.Controllers;
using BBK.SaaS.Mdls.Cms.Configuration;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
	[Area("App")]
	[AbpMvcAuthorize(AppPermissions.Pages_Administration_Tenant_Settings)]
	public class SettingsController : SaaSControllerBase
	{
		private readonly UserManager _userManager;
		private readonly TenantManager _tenantManager;
		private readonly IAppConfigurationAccessor _configurationAccessor;
		private readonly ITenantSettingsAppService _tenantSettingsAppService;
		private readonly IMultiTenancyConfig _multiTenancyConfig;
		private readonly ITimingAppService _timingAppService;

		public SettingsController(
			ITenantSettingsAppService tenantSettingsAppService,
			IMultiTenancyConfig multiTenancyConfig,
			ITimingAppService timingAppService,
			UserManager userManager,
			TenantManager tenantManager,
			IAppConfigurationAccessor configurationAccessor)
		{
			_tenantSettingsAppService = tenantSettingsAppService;
			_multiTenancyConfig = multiTenancyConfig;
			_timingAppService = timingAppService;
			_userManager = userManager;
			_tenantManager = tenantManager;
			_configurationAccessor = configurationAccessor;
		}

		public async Task<ActionResult> Index()
		{
			var output = await _tenantSettingsAppService.GetAllSettings();
			ViewBag.IsMultiTenancyEnabled = _multiTenancyConfig.IsEnabled;

			var timezoneItems = await _timingAppService.GetTimezoneComboboxItems(new GetTimezoneComboboxItemsInput
			{
				DefaultTimezoneScope = SettingScopes.Tenant,
				SelectedTimezoneId = await SettingManager.GetSettingValueForTenantAsync(TimingSettingNames.TimeZone, AbpSession.GetTenantId())
			});

			var user = await _userManager.GetUserAsync(AbpSession.ToUserIdentifier());

			ViewBag.CurrentUserEmail = user.EmailAddress;

			var tenant = await _tenantManager.FindByIdAsync(AbpSession.GetTenantId());
			ViewBag.TenantId = tenant.Id;
			ViewBag.TenantDarkLogoId = tenant.DarkLogoId;
			ViewBag.TenantLightLogoId = tenant.LightLogoId;
			ViewBag.TenantCustomCssId = tenant.CustomCssId;
			ViewBag.TenantWelcomeImg = output.UserManagement.WelcomeImageUrl;

			var model = new SettingsViewModel
			{
				Settings = output,
				TimezoneItems = timezoneItems

			};

			//model.CmsSettings = new Mdls.Cms.Configuration.Dto.CmsSettingsEditDto()
			//{
			//	General = new Mdls.Cms.Configuration.Dto.CmsGeneralSettingsEditDto()
			//	{
			//		FooterCusomHtml = await SettingManager.GetSettingValueForTenantAsync(CmsAppSettings.General.FooterCustomHTML, AbpSession.GetTenantId())
			//	}
			//};

			AddEnabledSocialLogins(model);

			return View(model);
		}

		private void AddEnabledSocialLogins(SettingsViewModel model)
		{
			if (!bool.Parse(_configurationAccessor.Configuration["Authentication:AllowSocialLoginSettingsPerTenant"]))
			{
				return;
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:Facebook:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("Facebook");
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:Google:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("Google");
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:Twitter:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("Twitter");
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:Microsoft:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("Microsoft");
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:OpenId:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("OpenId");
			}

			if (bool.Parse(_configurationAccessor.Configuration["Authentication:WsFederation:IsEnabled"]))
			{
				model.EnabledSocialLoginSettings.Add("WsFederation");
			}
		}
	}
}