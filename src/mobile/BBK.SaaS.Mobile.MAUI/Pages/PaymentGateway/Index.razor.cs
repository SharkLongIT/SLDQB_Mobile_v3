using BBK.SaaS.ApiClient;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.PaymentGateway
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        public Index() { }
        //private bool isError = false;

        private PaymentSelectModal payment { get; set; }

        public async Task OpenSelect()
        {
            await payment.OpenFor();
        }
    }

}
