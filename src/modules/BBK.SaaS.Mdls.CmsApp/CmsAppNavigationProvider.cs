using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Navigation;
using Abp.Localization;

namespace BBK.SaaS.Mdls.Cms
{
	public class CmsAppNavigationProvider : NavigationProvider
	{
		public const string MenuName = "ContentMgmtMenu";

		public override void SetNavigation(INavigationProviderContext context)
		{
			//var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Profile Menu"));
			var menu = new MenuDefinition(MenuName, new FixedLocalizableString("Content Management"));

			if (context.Manager.Menus.ContainsKey(MenuName))
			{
				menu = context.Manager.Menus[MenuName];
			}
			{
				menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Content Management"));
			}

			menu
				.AddItem(new MenuItemDefinition(
						CmsPageNames.Administrators.ContentManagementAdmin,
						L("Quản lý nội dung"),
						icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
					)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.Topics,
							new FixedLocalizableString("Quản lý trang tĩnh"), //L(CmsPageNames.Administrators.Topics),
							url: "/CMS/Topics",
							icon: "flaticon-users"
							//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
							)
						)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.Categories,
							new FixedLocalizableString("Quản lý chuyên mục"), //L(CmsPageNames.Administrators.Categories),
							url: "/CMS/Categories",
							icon: "flaticon-users"
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.Articles,
							new FixedLocalizableString("Quản lý bài viết"), //L(CmsPageNames.Administrators.Articles),
							url: "/CMS/Articles",
							icon: "flaticon-users"
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.MediasMgr,
							new FixedLocalizableString("Quản lý hình ảnh"),
							url: "/CMS/MediasMgr",
							icon: "flaticon-users"
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.UrlRecords,
							 new FixedLocalizableString("Quản lý links"), //L(CmsPageNames.Administrators.UrlRecords),
							url: "/CMS/UrlRecords",
							icon: "flaticon-users"
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
					//.AddItem(new MenuItemDefinition(
					//		CmsPageNames.Administrators.Widgets,
					//		 new FixedLocalizableString("Quản lý widgets"), //L(CmsPageNames.Administrators.UrlRecords),
					//		url: "/CMS/Widgets",
					//		icon: "flaticon-users"
					//	//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
					//	)
					//)
					//.AddItem(new MenuItemDefinition(
					//		CmsPageNames.Administrators.WidgetZones,
					//		 new FixedLocalizableString("Quản lý widgetZones"), //L(CmsPageNames.Administrators.UrlRecords),
					//		url: "/CMS/Widgets/WidgetZonesCfg",
					//		icon: "flaticon-users"
					//	//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
					//	)
					//)
					.AddItem(new MenuItemDefinition(
							CmsPageNames.Administrators.WidgetZones,
							 new FixedLocalizableString("Quản lý giao diện"), //L(CmsPageNames.Administrators.UrlRecords),
							url: "/CMS/Settings",
							icon: "flaticon-users"
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
