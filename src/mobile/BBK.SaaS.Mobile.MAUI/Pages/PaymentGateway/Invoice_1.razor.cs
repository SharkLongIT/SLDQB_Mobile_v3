using BBK.SaaS.Mobile.MAUI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.PaymentGateway
{
    public partial class Invoice_1 : SaaSMainLayoutPageComponentBase
    {
        public Invoice_1() { }
        private ModalBill_1 bill { get; set; }

        public async Task OpenBill()
        {
            await bill.OpenFor();
        }
    }
}
