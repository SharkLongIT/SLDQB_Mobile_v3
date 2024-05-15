namespace BBK.SaaS.Web.Areas.App.Startup
{
    public class AppPageNames
    {
        public static class Administrators
        {
            public const string Administrations = "Administrations";
            public const string Roles = "Administrations.Roles";
            public const string Users = "Administrations.Users";
            public const string AuditLogs = "Administrations.AuditLogs";
            public const string OrganizationUnits = "Administrations.OrganizationUnits";
            public const string Languages = "Administrations.Languages";
            public const string DemoUiComponents = "Administrations.DemoUiComponents";
            public const string UiCustomization = "Administrations.UiCustomization";
            public const string WebhookSubscriptions = "Administrations.WebhookSubscriptions";
            public const string DynamicProperties = "Administrations.DynamicProperties";
            public const string DynamicEntityProperties = "Administrations.DynamicEntityProperties";
            public const string Notifications = "Administrations.Notifications";
            public const string Notifications_Inbox = "Administrations.Notifications.Inbox";
            public const string Notifications_MassNotifications = "Administrations.Notifications.MassNotifications";
        }

        public static class Host
        {
            public const string Tenants = "Tenants";
            public const string Editions = "Editions";
            public const string Maintenance = "Administrations.Maintenance";
            public const string Settings = "Administrations.Settings.Host";
            public const string Dashboard = "Dashboard";
        }

        public static class Tenant
        {
            public const string Dashboard = "Dashboard.Tenant";
            public const string Settings = "Administrations.Settings.Tenant";
            public const string SubscriptionManagement = "Administrations.SubscriptionManagement.Tenant";
        }

        public static class Recruiter
        {
            public const string Recruiters = "Recruiters";
        }
    }
}
