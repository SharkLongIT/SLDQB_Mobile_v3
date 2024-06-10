using BBK.SaaS.Mobile.MAUI.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.PaymentGateway
{
    public partial class ModalBill_1 : ModalBase
    {
        public override string ModalId => "bill-1";

        public ModalBill_1() { }

        public async Task OpenFor()
        {
            await Show();
        }
    }
}
