using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Authorization.Accounts.Dto;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.LienHeHoiDap;
using BBK.SaaS.Mobile.MAUI.Models.User;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Account;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.Register
{
    public partial class Index : SaaSComponentBase
    {
        public string UserName
        {
            get => _accountService.AbpAuthenticateModel.UserNameOrEmailAddress;
            set
            {
                _accountService.AbpAuthenticateModel.UserNameOrEmailAddress = value;
            }
        }

        public string Password
        {
            get => _accountService.AbpAuthenticateModel.Password;
            set
            {
                _accountService.AbpAuthenticateModel.Password = value;
            }
        }
        protected UserEditModel Model = new UserEditModel();
        private IAccountService _accountService;
        private IAccountAppService _accountAppService;
        private IApplicationContext _applicationContext;

        private IUserTypeAppService userTypeAppService;
       

        public string CurrentTenancyNameOrDefault => _applicationContext.CurrentTenant != null
        ? _applicationContext.CurrentTenant.TenancyName
        : L("Chưa chọn");

        int tenantId;
        public Index()
        {
            _accountService = DependencyResolver.Resolve<IAccountService>();
            _accountAppService = DependencyResolver.Resolve<IAccountAppService>();
            _applicationContext = DependencyResolver.Resolve<IApplicationContext>();
        }
      
        private async Task RegisterUser()
        {
            try
            {
                tenantId = 1;
                await SetBusyAsync(async () =>
                {
                    //if (!await ValidateInput())
                    //{
                    //    return;
                    //}
                    await WebRequestExecuter.Execute(
                      async () => await userTypeAppService.Register(tenantId ,Model),
                      async () =>
                      {
                          //Model = new ContactModel();
                          await UserDialogsService.AlertSuccess(L("Đăng ký tài khoản thành công!"));
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
