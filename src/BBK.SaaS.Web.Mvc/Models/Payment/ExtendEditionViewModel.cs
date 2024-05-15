using System.Collections.Generic;
using BBK.SaaS.Editions.Dto;
using BBK.SaaS.MultiTenancy.Payments;

namespace BBK.SaaS.Web.Models.Payment
{
    public class ExtendEditionViewModel
    {
        public EditionSelectDto Edition { get; set; }

        public List<PaymentGatewayModel> PaymentGateways { get; set; }
    }
}