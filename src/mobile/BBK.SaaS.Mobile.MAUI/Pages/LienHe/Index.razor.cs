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
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

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
        private async Task<bool> ValidateInput()
        {

            //IsUserLoggedIn = navigationService.IsUserLoggedIn();

            //if (IsUserLoggedIn == false)
            //{
            //    await UserDialogsService.AlertWarn("Bạn chưa đăng nhập!");
            //    ReturnLogin = true;
            //    return false;
            //}
            if (Model.FullName == null || string.IsNullOrEmpty(Model.FullName))
            {
                await UserDialogsService.AlertWarn("Họ và tên không được để trống!");
                return false;
            }
            if (Model.Email == null || string.IsNullOrEmpty(Model.Email))
            {
                await UserDialogsService.AlertWarn("Email không được để trống!");
                return false;
            }
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            if (!Regex.IsMatch(Model.Email, emailPattern))
            {
                await UserDialogsService.AlertWarn("Email không hợp lệ!");
                return false;
            }
            if (Model.Phone == null || string.IsNullOrEmpty(Model.Phone))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không được để trống!");
                return false;
            }
            string phonePattern = @"^0[0-9]{9,10}$";
            if (!Regex.IsMatch(Model.Phone, phonePattern))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không hợp lệ! Vui lòng kiểm tra lại.");
                return false;
            }
            if (Model.Description == null || string.IsNullOrEmpty(Model.Description))
            {
                await UserDialogsService.AlertWarn("Nội dung không được để trống!");
                return false;
            }

            return true;

        }

        private async Task SendMail()
        {
            if (ApplicationContext.LoginInfo == null)
            {

                Model.CreatorUserId = null;
            }
            else
            {
                Model.CreatorUserId = ApplicationContext.LoginInfo.User.Id;
            }
            try
            {
                await SetBusyAsync(async () =>
                {
                    if (!await ValidateInput())
                    {
                        return;
                    }
                    await WebRequestExecuter.Execute(
                      async () => await contactAppService.Create(Model),
                      async (result) =>
                      {
                          Model = new ContactModel();
                          await UserDialogsService.AlertSuccess(L("Cảm ơn bạn đã đóng góp ý kiến!"));
                          //await Hide();
                          //await OnSave.InvokeAsync();
                      }
                   ); ;
                });


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }


        }
    }
}
