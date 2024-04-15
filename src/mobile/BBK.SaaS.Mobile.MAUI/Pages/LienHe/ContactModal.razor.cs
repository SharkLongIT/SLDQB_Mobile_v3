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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Text.RegularExpressions;

namespace BBK.SaaS.Mobile.MAUI.Pages.LienHe
{
    public partial class ContactModal : ModalBase
    {

        public override string ModalId => "contact";
        [Parameter] public EventCallback OnSave { get; set; }

        protected NavMenu NavMenu { get; set; }
        protected IContactAppService contactAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IApplicationContext ApplicationContext;

        private ItemsProviderResult<ContactDto> contacDto;
        protected UserDialogsService UserDialogsService { get; set; }
        private bool _isInitialized;
        private bool IsUserLoggedIn;
        private bool ReturnLogin;
        protected ContactModel Model = new();
        public ContactModal()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            contactAppService = DependencyResolver.Resolve<IContactAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();

        }

        public async Task OpenFor(ContactModal contact)
        {

            _isInitialized = false;
            try
            {
                await SetBusyAsync(async () =>
                {
                    Model = new ContactModel();
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                            _isInitialized = true;
                        }
                    );

                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            await Show();
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
                          await Hide();
                          await OnSave.InvokeAsync();
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
