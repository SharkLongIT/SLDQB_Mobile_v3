using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class PaymentSelectModal : ModalBase
    {
        public override string ModalId => "payment-select";
        protected DatLichModel UserInput;
        [Parameter] public EventCallback<string> OnSave { get; set; }
        public PaymentSelectModal() { }
        public async Task OpenFor(DatLichModel datLichModel)
        {
            await Show();
        }
    }
}
