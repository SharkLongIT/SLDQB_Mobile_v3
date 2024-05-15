//using System.Collections.Generic;
//using BBK.SaaS.Web.DashboardCustomization;


//namespace BBK.SaaS.Web.Areas.App.Startup
//{
//    public class DashboardViewConfiguration
//    {
//        public Dictionary<string, WidgetViewDefinition> WidgetViewDefinitions { get; } = new Dictionary<string, WidgetViewDefinition>();

//        public Dictionary<string, WidgetFilterViewDefinition> WidgetFilterViewDefinitions { get; } = new Dictionary<string, WidgetFilterViewDefinition>();

//        public DashboardViewConfiguration()
//        {
//            var jsAndCssFileRoot = "/Areas/App/Views/CustomizableDashboard/Widgets/";
//            var viewFileRoot = "App/Widgets/";

//            #region FilterViewDefinitions

//            WidgetFilterViewDefinitions.Add(SaaSDashboardCustomizationConsts.Filters.FilterDateRangePicker,
//                new WidgetFilterViewDefinition(
//                    SaaSDashboardCustomizationConsts.Filters.FilterDateRangePicker,
//                    "~/Areas/App/Views/Shared/Components/CustomizableDashboard/Widgets/DateRangeFilter.cshtml",
//                    jsAndCssFileRoot + "DateRangeFilter/DateRangeFilter.min.js",
//                    jsAndCssFileRoot + "DateRangeFilter/DateRangeFilter.min.css")
//            );
            
//            //add your filters iew definitions here
//            #endregion

//            #region WidgetViewDefinitions

//            #region TenantWidgets

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.DailySales,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.DailySales,
//                    viewFileRoot + "DailySales",
//                    jsAndCssFileRoot + "DailySales/DailySales.min.js",
//                    jsAndCssFileRoot + "DailySales/DailySales.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.GeneralStats,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.GeneralStats,
//                    viewFileRoot + "GeneralStats",
//                    jsAndCssFileRoot + "GeneralStats/GeneralStats.min.js",
//                    jsAndCssFileRoot + "GeneralStats/GeneralStats.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.ProfitShare,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.ProfitShare,
//                    viewFileRoot + "ProfitShare",
//                    jsAndCssFileRoot + "ProfitShare/ProfitShare.min.js",
//                    jsAndCssFileRoot + "ProfitShare/ProfitShare.min.css"));
  
//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.MemberActivity,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.MemberActivity,
//                    viewFileRoot + "MemberActivity",
//                    jsAndCssFileRoot + "MemberActivity/MemberActivity.min.js",
//                    jsAndCssFileRoot + "MemberActivity/MemberActivity.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.RegionalStats,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.RegionalStats,
//                    viewFileRoot + "RegionalStats",
//                    jsAndCssFileRoot + "RegionalStats/RegionalStats.min.js",
//                    jsAndCssFileRoot + "RegionalStats/RegionalStats.min.css",
//                    12,
//                    10));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.SalesSummary,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.SalesSummary,
//                    viewFileRoot + "SalesSummary",
//                    jsAndCssFileRoot + "SalesSummary/SalesSummary.min.js",
//                    jsAndCssFileRoot + "SalesSummary/SalesSummary.min.css",
//                    6,
//                    10));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Tenant.TopStats,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Tenant.TopStats,
//                    viewFileRoot + "TopStats",
//                    jsAndCssFileRoot + "TopStats/TopStats.min.js",
//                    jsAndCssFileRoot + "TopStats/TopStats.min.css",
//                    12,
//                    10));

//            //add your tenant side widget definitions here
//            #endregion

//            #region HostWidgets

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Host.IncomeStatistics,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Host.IncomeStatistics,
//                    viewFileRoot + "IncomeStatistics",
//                    jsAndCssFileRoot + "IncomeStatistics/IncomeStatistics.min.js",
//                    jsAndCssFileRoot + "IncomeStatistics/IncomeStatistics.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Host.TopStats,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Host.TopStats,
//                    viewFileRoot + "HostTopStats",
//                    jsAndCssFileRoot + "HostTopStats/HostTopStats.min.js",
//                    jsAndCssFileRoot + "HostTopStats/HostTopStats.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Host.EditionStatistics,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Host.EditionStatistics,
//                    viewFileRoot + "EditionStatistics",
//                    jsAndCssFileRoot + "EditionStatistics/EditionStatistics.min.js",
//                    jsAndCssFileRoot + "EditionStatistics/EditionStatistics.min.css"));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Host.SubscriptionExpiringTenants,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Host.SubscriptionExpiringTenants,
//                    viewFileRoot + "SubscriptionExpiringTenants",
//                    jsAndCssFileRoot + "SubscriptionExpiringTenants/SubscriptionExpiringTenants.min.js",
//                    jsAndCssFileRoot + "SubscriptionExpiringTenants/SubscriptionExpiringTenants.min.css",
//                    6,
//                    10));

//            WidgetViewDefinitions.Add(SaaSDashboardCustomizationConsts.Widgets.Host.RecentTenants,
//                new WidgetViewDefinition(
//                    SaaSDashboardCustomizationConsts.Widgets.Host.RecentTenants,
//                    viewFileRoot + "RecentTenants",
//                    jsAndCssFileRoot + "RecentTenants/RecentTenants.min.js",
//                    jsAndCssFileRoot + "RecentTenants/RecentTenants.min.css"));

//            //add your host side widgets definitions here
//            #endregion

//            #endregion
//        }
//    }
//}
