using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.PaymentGateway
{
    public partial class PaymentSelectModal : ModalBase
    {
        public override string ModalId => "payment-select";
        //protected DatLichModel UserInput;
    
        [Parameter] public EventCallback<string> OnSave { get; set; }
        public PaymentSelectModal() { }

        public async Task OpenFor()
        {
            await Show();
        }
    }
}
