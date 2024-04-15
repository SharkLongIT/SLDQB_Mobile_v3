using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using BBK.SaaS.Mdls.Profile.Authorization;

namespace BBK.SaaS.Mdls.Profile
{
    public class SaaSProfileAppNavigationProvider : NavigationProvider
	{
		public const string MenuName = "ProfileMenu";

		public override void SetNavigation(INavigationProviderContext context)
		{
			//var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Profile Menu"));
			var menu = new MenuDefinition(MenuName, new FixedLocalizableString("Profile Menu"));
			
			if (context.Manager.Menus.ContainsKey(MenuName))
			{
				menu = context.Manager.Menus[MenuName];
			}
			{
				menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Profile Menu"));
			}

			menu
				.AddItem(new MenuItemDefinition(
						"Profile.AccountManagement",
						new FixedLocalizableString("Quản lý SLĐ"),
						icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
					).AddItem(new MenuItemDefinition(
							"Profile.AccountManagement.UserType1",
							new FixedLocalizableString("Nhà tuyển dụng"),
							url: "/Profile/UsersType/IndexRecruiters",
							icon: "flaticon-users"
						//permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
						)
					)
                     .AddItem(new MenuItemDefinition(
                            "Profile.JobExchangePlatform.JobTransactionSession",
                            new FixedLocalizableString("Quản lý Tin tuyển dụng"),
                            url: "/Profile/Recruitments/NVNVRecruiment",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
                        )
                    ).AddItem(new MenuItemDefinition(
							"Profile.AccountManagement.UserType2",
							new FixedLocalizableString("Người lao động"),
							url: "/Profile/UsersType/IndexCandidates",
							icon: "flaticon-users"
                            //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
						)
					).AddItem(new MenuItemDefinition(
                            "Profile.AccountManagement.JobApplication",
                            new FixedLocalizableString("Hồ sơ tuyển dụng"),
                            url: "/Profile/UsersType1/CurriculumVitaeJobApplication",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
                        )
                    ).AddItem(new MenuItemDefinition(
							"Profile.KnowledgeMgmt.QnAMgmt",
							new FixedLocalizableString("Quản lý hỏi đáp"),
							url: "/App/ContactManager",
							icon: "flaticon-users"
                            //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
						)
					)
                    .AddItem(new MenuItemDefinition(
                            "Profile.JobExchangePlatform.JobTransactionSession",
                            new FixedLocalizableString("Quản lý Liên kết giới thiệu"),
                            url: "/Profile/Introduce",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
                        )
                    )
                      .AddItem(new MenuItemDefinition(
                            "Profile.JobExchangePlatform.JobTransactionSession",
                            new FixedLocalizableString("Quản lý Phiên giao dịch"),
                            url: "/Profile/TradingSession",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Candidate)
                        )
                    )
                )
                .AddItem(new MenuItemDefinition(
                        "Profile.Reportings",
                        new FixedLocalizableString("Thống kê & báo cáo"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                            "Profile.Reportings.ActivityStatistics",
                            new FixedLocalizableString("Hoạt động của Website"),
                            url: "/Profile/Report",
                            icon: "flaticon-users"
                            //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Recruiter)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Profile.Reportings.ActivityStatistics",
                            new FixedLocalizableString("Thống kê theo lĩnh vực"),
                            url: "/Profile/Report/ReportCat",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Recruiter)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            "Profile.Reportings.ActivityStatistics",
                            new FixedLocalizableString("Thống Kê Số Lượng Tin Tức"),
                            url: "/Profile/Report/ReportArticle",
                            icon: "flaticon-users"
                        //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Recruiter)
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
