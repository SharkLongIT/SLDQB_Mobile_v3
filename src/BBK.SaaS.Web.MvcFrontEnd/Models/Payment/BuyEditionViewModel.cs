using System.Collections.Generic;
using BBK.SaaS.Editions;
using BBK.SaaS.Editions.Dto;
using BBK.SaaS.MultiTenancy.Payments;
using BBK.SaaS.MultiTenancy.Payments.Dto;

namespace BBK.SaaS.Web.Models.Payment
{
    public class BuyEditionViewModel
    {
        public SubscriptionStartType? SubscriptionStartType { get; set; }

        public EditionSelectDto Edition { get; set; }

        public decimal? AdditionalPrice { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }

        public List<PaymentGatewayModel> PaymentGateways { get; set; }
    }
}
