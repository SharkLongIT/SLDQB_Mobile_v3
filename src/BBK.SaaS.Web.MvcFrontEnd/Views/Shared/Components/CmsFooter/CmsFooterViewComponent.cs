using Abp;
using Abp.Application.Navigation;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;
using BBK.SaaS.Configuration;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Views.Shared.Components.CmsFooter
{
	public class CmsFooterViewComponent : SaaSViewComponent
	{
		private readonly IUserNavigationManager _userNavigationManager;
		private readonly IAbpSession _abpSession;
		private readonly IConfiguration _configuration;
		private readonly ILocalizationContext _localizationContext;
		private readonly ICmsCatsAppService _catsAppService;
		private readonly IFECntCategoryAppService _feContentCtgAppService;

		public CmsFooterViewComponent(
			IConfiguration configuration,
			ILocalizationContext localizationContext,
			IUserNavigationManager userNavigationManager,
			ICmsCatsAppService catsAppService,
			IFECntCategoryAppService feContentCtgAppService,
			IAbpSession abpSession)
		{
			_configuration = configuration;
			_localizationContext = localizationContext;
			_userNavigationManager = userNavigationManager;
			_catsAppService = catsAppService;
			_feContentCtgAppService = feContentCtgAppService;
			_abpSession = abpSession;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			CmsFooterViewModel model = new CmsFooterViewModel();

			model.FooterHtml = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.FooterCustomHTML);
			if (!string.IsNullOrEmpty(model.FooterHtml))
			{
				model.IsUseCustomFooter = true;

			}

			//new CmsGeneralSettingsEditDto()
			//	{
			//		FooterCusomHtml = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.FooterCustomHTML),
			//		HeaderCusomHtml = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.HeaderCustomHTML)
			//	},

			return View(model);
		}
	}
}
