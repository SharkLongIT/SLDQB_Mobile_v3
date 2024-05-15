using Abp.Application.Navigation;
using Abp.Runtime.Session;
using BBK.SaaS.Configuration;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Components.AppSideBar
{
	public class AppSideBarViewComponent : SaaSViewComponent
	{
		private readonly IUserNavigationManager _userNavigationManager;
		private readonly IAbpSession _abpSession;
		private readonly IConfiguration _configuration;
		//private readonly INavigationProviderContext _navCtx;

		public AppSideBarViewComponent(
			IConfiguration configuration,
			IUserNavigationManager userNavigationManager,
			//INavigationProviderContext navCtx,
			IAbpSession abpSession)
		{
			_configuration = configuration;
			_userNavigationManager = userNavigationManager;
			//_navCtx = navCtx;
			_abpSession = abpSession;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			var layoutType = await SettingManager.GetSettingValueAsync(AppSettings.UserManagement.AllowSelfRegistration);
			Logger.Info($"MYLAYOUTIS:{layoutType}");

			//_configuration.Get
			var model = new AppNavigationViewModel
			{
				//MainMenu = await _userNavigationManager.GetMenuAsync("MainMenu", _abpSession.ToUserIdentifier())
				AppMenu = await _userNavigationManager.GetMenuAsync(AppNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
				//MegaMenu = await _userNavigationManager.GetMenuAsync(ProfileNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
			};
			model.Menus = new System.Collections.Generic.List<UserMenu>();

			foreach (var menu in await _userNavigationManager.GetMenusAsync(_abpSession.ToUserIdentifier()))
			{
				if (menu.Name != AppNavigationProvider.MenuName && menu.Name != "MainMenu")
				{
					model.Menus.Add(menu);
				}
			}
			//_navCtx.Manager.MainMenu

			return View(model);
		}
	}
}
