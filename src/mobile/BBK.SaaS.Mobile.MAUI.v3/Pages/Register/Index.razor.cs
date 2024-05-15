using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Authorization.Accounts.Dto;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.LienHeHoiDap;
using BBK.SaaS.Mobile.MAUI.Models.User;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Account;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.RegularExpressions;

namespace BBK.SaaS.Mobile.MAUI.Pages.Register
{
    public partial class Index : SaaSComponentBase
    {
      

        protected RegisterInput Model = new RegisterInput();
        protected IUserAppService UserAppService;
        protected ProxyAccountAppService ProxyAccountAppService;
        protected IUserTypeAppService UserTypeAppService;
        public Index()
        {
            UserAppService = DependencyResolver.Resolve<IUserAppService>();
            ProxyAccountAppService = DependencyResolver.Resolve<ProxyAccountAppService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await JS.InvokeVoidAsync("checkPassword");
        }
        private async Task<bool> ValidateInput()
        {
            if (Model.Name == null || string.IsNullOrEmpty(Model.Name))
            {
                await UserDialogsService.AlertWarn("Tên không được để trống!");
                return false;
            }
            if (Model.EmailAddress == null || string.IsNullOrEmpty(Model.EmailAddress))
            {
                await UserDialogsService.AlertWarn("Email không được để trống!");
                return false;
            }
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            if (!Regex.IsMatch(Model.EmailAddress, emailPattern))
            {
                await UserDialogsService.AlertWarn("Email không hợp lệ!");
                return false;
            }
            if (Model.Password == null || string.IsNullOrEmpty(Model.Password))
            {
                await UserDialogsService.AlertWarn("Mật khẩu không được để trống!");
                return false;
            }
            if (Model.Password.Length < 6)
            {
                await UserDialogsService.AlertWarn("Mật khẩu phải chứa ít nhất 6 ký tự!");
                return false;
            }
            return true;
        }
        private async Task RegisterUser()
        {
            try
            {
                Model.UserName = Model.EmailAddress;
                Model.Surname = "?";
                await SetBusyAsync(async () =>
                {
                    if (!await ValidateInput())
                    {
                        return;
                    }
                    await WebRequestExecuter.Execute(
                      //async () => await ProxyAccountAppService.Register(Model),
                      async () => await ProxyAccountAppService.Register(Model),
                      async () =>
                      {
                          await UserDialogsService.AlertSuccess(L("Đăng ký tài khoản thành công!"));
                         
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
