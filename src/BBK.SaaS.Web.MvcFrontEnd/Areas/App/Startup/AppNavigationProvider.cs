using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Localization;
using BBK.SaaS.Authorization;

namespace BBK.SaaS.Web.Areas.App.Startup
{
    public class AppNavigationProvider : NavigationProvider
    {
        public const string MenuName = "Administrators";

        public override void SetNavigation(INavigationProviderContext context)
        {
            var menu = context.Manager.Menus[MenuName] = new MenuDefinition(MenuName, new FixedLocalizableString("Main Menu"));

            menu
                .AddItem(new MenuItemDefinition(
                        AppPageNames.Host.Dashboard,
                        L("Dashboard"),
                        url: "/App/HostDashboard",
                        icon: "flaticon-line-graph",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Host_Dashboard)
                    )
                ).AddItem(new MenuItemDefinition(
                        AppPageNames.Host.Tenants,
                        L("Tenants"),
                        url: "/App/Tenants",
                        icon: "flaticon-list-3",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenants)
                    )
                ).AddItem(new MenuItemDefinition(
                        AppPageNames.Host.Editions,
                        L("Editions"),
                        url: "/App/Editions",
                        icon: "flaticon-app",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Editions)
                    )
                ).AddItem(new MenuItemDefinition(
                        AppPageNames.Tenant.Dashboard,
                        L("Dashboard"),
                        url: "/App/TenantDashboard",
                        icon: "flaticon-line-graph",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Tenant_Dashboard)
                    )
                ).AddItem(new MenuItemDefinition(
                        AppPageNames.Administrators.Administrations,
                        L("Administration"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.OrganizationUnits,
                            L("OrganizationUnits"),
                            url: "/App/OrganizationUnits",
                            icon: "flaticon-map",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_OrganizationUnits)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.Roles,
                            L("Roles"),
                            url: "/App/Roles",
                            icon: "flaticon-suitcase",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Roles)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.Users,
                            L("Users"),
                            url: "/App/Users",
                            icon: "flaticon-users",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.Languages,
                            L("Languages"),
                            url: "/App/Languages",
                            icon: "flaticon-tabs",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Languages)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.AuditLogs,
                            L("AuditLogs"),
                            url: "/App/AuditLogs",
                            icon: "flaticon-folder-1",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_AuditLogs)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Host.Maintenance,
                            L("Maintenance"),
                            url: "/App/Maintenance",
                            icon: "flaticon-lock",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Host_Maintenance)
                        )
                    ).AddItem(new MenuItemDefinition(
                            AppPageNames.Tenant.SubscriptionManagement,
                            L("Subscription"),
                            url: "/App/SubscriptionManagement",
                            icon: "flaticon-refresh",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Tenant_SubscriptionManagement)
                        )
                    )
                    //.AddItem(new MenuItemDefinition(
                    //        AppPageNames.Administrators.UiCustomization,
                    //        L("VisualSettings"),
                    //        url: "/App/UiCustomization",
                    //        icon: "flaticon-medical",
                    //        permissionDependency: new SimplePermissionDependency(AppPermissions
                    //            .Pages_Administration_UiCustomization)
                    //    )
                    //)
                    //.AddItem(new MenuItemDefinition(
                    //        AppPageNames.Administrators.WebhookSubscriptions,
                    //        L("WebhookSubscriptions"),
                    //        url: "/App/WebhookSubscription",
                    //        icon: "flaticon2-world",
                    //        permissionDependency: new SimplePermissionDependency(AppPermissions
                    //            .Pages_Administration_WebhookSubscription)
                    //    )
                    //)
                    //.AddItem(new MenuItemDefinition(
                    //        AppPageNames.Administrators.DynamicProperties,
                    //        L("DynamicProperties"),
                    //        url: "/App/DynamicProperty",
                    //        icon: "flaticon-interface-8",
                    //        permissionDependency: new SimplePermissionDependency(AppPermissions
                    //            .Pages_Administration_DynamicProperties)
                    //    )
                    //)
                    .AddItem(new MenuItemDefinition(
                            AppPageNames.Host.Settings,
                            L("Settings"),
                            url: "/App/HostSettings",
                            icon: "flaticon-settings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Host_Settings)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            AppPageNames.Tenant.Settings,
                            L("Settings"),
                            url: "/App/Settings",
                            icon: "flaticon-settings",
                            permissionDependency: new SimplePermissionDependency(AppPermissions
                                .Pages_Administration_Tenant_Settings)
                        )
                    )
                    .AddItem(new MenuItemDefinition(
                            AppPageNames.Administrators.Notifications,
                            L("Notifications"),
                            icon: "flaticon-alarm"
                        ).AddItem(new MenuItemDefinition(
                                AppPageNames.Administrators.Notifications_Inbox,
                                L("Inbox"),
                                url: "/App/Notifications",
                                icon: "flaticon-mail-1"
                            )
                        )
                        .AddItem(new MenuItemDefinition(
                                AppPageNames.Administrators.Notifications_MassNotifications,
                                L("MassNotifications"),
                                url: "/App/Notifications/MassNotifications",
                                icon: "flaticon-paper-plane",
                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_MassNotification)
                            )
                        )
                    )
                ).AddItem(new MenuItemDefinition(
                        AppPageNames.Administrators.DemoUiComponents,
                        L("DemoUiComponents"),
                        url: "/App/DemoUiComponents",
                        icon: "flaticon-shapes",
                        permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_DemoUiComponents)
                    )

                )
                .AddItem(new MenuItemDefinition(
                        AppPageNames.Administrators.DemoUiComponents,
                        L("Quản trị hỏi đáp"),
                        url: "/App/ContactManager",
                        icon: "flaticon-users"
                    //, permissionDependency: new SimplePermissionDependency(ProfilePermission.Recruiter)
                    )
                 .AddItem(new MenuItemDefinition(
                                AppPageNames.Administrators.Notifications_MassNotifications,
                                 L("Quản trị hỏi đáp"),
                                 url: "/App/ContactManager",
                                icon: "flaticon-paper-plane",
                                permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_MassNotification)
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
