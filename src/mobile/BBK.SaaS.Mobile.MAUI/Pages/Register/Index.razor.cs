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

namespace BBK.SaaS.Mobile.MAUI.Pages.Register
{
    public partial class Index : SaaSComponentBase
    {
        //protected UserCreateOrUpdateModel Model = new();
     protected RegisterInput Model = new RegisterInput();

        protected UserEditDto UserInput = new UserEditDto();
        protected IUserAppService UserAppService;
        protected ProxyAccountAppService ProxyAccountAppService;
        protected IUserTypeAppService UserTypeAppService;
        public Index()
        {
            UserAppService = DependencyResolver.Resolve<IUserAppService>();
            ProxyAccountAppService = DependencyResolver.Resolve<ProxyAccountAppService>();
            UserTypeAppService = DependencyResolver.Resolve<IUserTypeAppService>();
        }
        public async Task IsUserType1()
        {
            //Model.User.UserType = UserTypeEnum.Type1;
        }
        public async Task IsUserType2()
        {
            //Model.User.UserType = UserTypeEnum.Type2;
        }
        int tenantId;
        private async Task RegisterUser()
        {
            try
            {
                tenantId = 1;
                UserInput.Name = "Pham Van Dong";
                UserInput.UserType = UserTypeEnum.Type1;
                UserInput.Password = "123qwe";
                UserInput.EmailAddress = "dongpham@gmail.com";

                //Model.UserName = Model.EmailAddress;
                //Model.Surname = "Phạm";
                await SetBusyAsync(async () =>
                {
                    //if (!await ValidateInput())
                    //{
                    //    return;
                    //}
                    await WebRequestExecuter.Execute(
                      async () => await UserTypeAppService.Register(tenantId, UserInput),
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
