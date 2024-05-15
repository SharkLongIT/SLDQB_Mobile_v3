using BBK.SaaS.MultiTenancy.Payments;

namespace BBK.SaaS.Web.Models.Payment
{
    public class CancelPaymentModel
    {
        public string PaymentId { get; set; }

        public SubscriptionPaymentGatewayType Gateway { get; set; }
    }
}