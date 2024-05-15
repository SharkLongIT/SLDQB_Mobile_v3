using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.ProfileNTD;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class ViewContactModal : ModalBase
    {
        [Parameter] public EventCallback<string> OnSave { get; set; }
        public override string ModalId => "Detail-Contact";
        protected IContactAppService contactAppService;
        protected ContactDto Model = new();

        public ViewContactModal()
        {

            contactAppService = DependencyResolver.Resolve<IContactAppService>();
        }
        protected virtual async Task Cancel()
        {
            await Hide();
        }
        public async Task OpenFor(ContactDto contact)
        {

            try
            {

                Model = contact;

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

            await Show();
        }

    }
}
