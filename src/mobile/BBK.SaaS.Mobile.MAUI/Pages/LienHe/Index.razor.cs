using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mobile.MAUI.Models.Contact;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.LienHe
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {

        protected NavMenu NavMenu { get; set; }
        protected IContactAppService contactAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IApplicationContext ApplicationContext;

        private ItemsProviderResult<ContactDto> contacDto;
        protected UserDialogsService UserDialogsService { get; set; }

        protected ContactModel Model = new();
        private Virtualize<ContactDto> ContactContainer { get; set; }
        private bool IsUserLoggedIn;

        public Index()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            contactAppService = DependencyResolver.Resolve<IContactAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();

        }
        protected override async Task OnInitializedAsync()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();

            await SetPageHeader(L("Liên hệ hỏi đáp"));
        }
        private async Task RefreshList()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
        private ContactModal contactModal { get; set; }
        public async Task OpenContactModal()
        {
            await contactModal.OpenFor(null);
        }
        public async Task Login()
        {
            navigationService.NavigateTo(NavigationUrlConsts.Login);
        }
    }
}
