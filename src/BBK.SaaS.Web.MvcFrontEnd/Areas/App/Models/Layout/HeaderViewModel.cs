using BBK.SaaS.Sessions.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Layout
{
    public class HeaderViewModel
    {
        public int SubscriptionExpireNotifyDayCount { get; set; }

        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }

        public string GetLogoUrl(string appPath, string logoSkin)
        {
            //if (LoginInformations?.Tenant == null || !LoginInformations.Tenant.HasLogo())
            //{
            //    return appPath + "Common/Images/app-logo-on-" + logoSkin + ".svg";
            //}

            //id parameter is used to prevent caching only.
            //return appPath + "TenantCustomization/GetTenantLogo?tenantId=" + LoginInformations?.Tenant?.Id + "&skin=" + logoSkin;
            return appPath + "TenantCustomization/GetTenantLogo?tenantId=1&skin=" + logoSkin;
        }
    }
}
