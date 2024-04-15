using Abp.Application.Navigation;
using Abp.Localization;

namespace BBK.SaaS.Mdls.Category
{
	public class SaaSCategoryAppNavigationProvider : NavigationProvider
	{
		//public const string MenuName = "";
		

		public override void SetNavigation(INavigationProviderContext context)
		{
			//var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Profile Menu"));
			var menu = new MenuDefinition(SaaSConsts.AppAdminMenuName, new FixedLocalizableString(SaaSConsts.AppAdminMenuName));
			
			if (context.Manager.Menus.ContainsKey(SaaSConsts.AppAdminMenuName))
			{
				menu = context.Manager.Menus[SaaSConsts.AppAdminMenuName];
			}
			else
			{
				menu = context.Manager.Menus[SaaSConsts.AppAdminMenuName] = new MenuDefinition(SaaSConsts.AppAdminMenuName, new FixedLocalizableString(SaaSConsts.AppAdminMenuName));
			}

			menu
				.AddItem(new MenuItemDefinition(
						"Categories.GeoManagement",
						new FixedLocalizableString("Quản lý danh mục"),
						icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
					).AddItem(new MenuItemDefinition(
							"Categories.GeoManagement.List",
							new FixedLocalizableString("Địa chỉ"),
							url: "/Lib/GeoUnits" /*{Area}/{Controller}/Index*/
							//icon: "flaticon-users",
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					).AddItem(new MenuItemDefinition(
							"Categories.GeoManagement.List",
							new FixedLocalizableString("Danh mục chung"),
							url: "/Lib/Indexings" /*{Area}/{Controller}/Index*/
						//icon: "flaticon-users",
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
				);
		}

		private static ILocalizableString L(string name)
		{
			return new LocalizableString(name, SaaSConsts.LocalizationSourceName);
		}
	}
}
