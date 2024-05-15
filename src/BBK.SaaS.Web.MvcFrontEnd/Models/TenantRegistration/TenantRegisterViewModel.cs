using BBK.SaaS.Editions;
using BBK.SaaS.Editions.Dto;
using BBK.SaaS.MultiTenancy.Payments;
using BBK.SaaS.Security;
using BBK.SaaS.MultiTenancy.Payments.Dto;

namespace BBK.SaaS.Web.Models.TenantRegistration
{
    public class TenantRegisterViewModel
    {
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public int? EditionId { get; set; }

        public SubscriptionStartType? SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }
    }
}
