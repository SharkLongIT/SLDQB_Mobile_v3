using Abp.Dependency;
using BBK.SaaS.ApiClient;
using BBK.SaaS.ApiClient.Models;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Localization;
using BBK.SaaS.Mobile.MAUI.Services.Account;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Services.Storage;
using BBK.SaaS.Sessions;
using BBK.SaaS.Sessions.Dto;

namespace BBK.SaaS.Services.Account
{
    public class AccountService : IAccountService, ISingletonDependency
    {
        private readonly IApplicationContext _applicationContext;
        private readonly ISessionAppService _sessionAppService;
        private readonly IAccessTokenManager _accessTokenManager;
        private readonly IDataStorageService _dataStorageService;
        private readonly INavigationService _navigationService;
        private readonly IApplicationContext AppContext;

        public AccountService(
            IApplicationContext applicationContext,
            ISessionAppService sessionAppService,
            IAccessTokenManager accessTokenManager,
            AbpAuthenticateModel abpAuthenticateModel,
            IDataStorageService dataStorageService,
            INavigationService navigationService,
            IApplicationContext AppContext
            )
        {
            _applicationContext = applicationContext;
            _sessionAppService = sessionAppService;
            _accessTokenManager = accessTokenManager;
            _dataStorageService = dataStorageService;
            AbpAuthenticateModel = abpAuthenticateModel;
            _navigationService = navigationService;
            this.AppContext = AppContext;
        }

        public AbpAuthenticateModel AbpAuthenticateModel { get; set; }
        public AbpAuthenticateResultModel AuthenticateResultModel { get; set; }

        public AbpSignUpModel AbpSignUpModel { get; set; }
        public AbpSignUpResultModel AbpSignUpResultModel { get; set; }

        public async Task LoginUserAsync()
        {
            await WebRequestExecuter.Execute(_accessTokenManager.LoginAsync, AuthenticateSucceed, ex => Task.CompletedTask);
        }

        public Task LogoutAsync()
        {
            _accessTokenManager.Logout();
            _applicationContext.ClearLoginInfo();
            _dataStorageService.ClearSessionPersistance();
            return Task.CompletedTask;
        }

        private async Task AuthenticateSucceed(AbpAuthenticateResultModel result)
        {
            AuthenticateResultModel = result;

            if (AuthenticateResultModel.ShouldResetPassword)
            {
                await UserDialogsService.Instance.AlertError(L.Localize("Đăng nhập thất bại") + " " + L.Localize("Thay đổi mật khẩu để đăng nhập"));
                return;
            }

            if (AuthenticateResultModel.RequiresTwoFactorVerification)
            {
                _navigationService.NavigateTo(NavigationUrlConsts.SendTwoFactorCode);
                return;
            }

            await _dataStorageService.StoreAuthenticateResultAsync(AuthenticateResultModel);

            AbpAuthenticateModel.Password = null;
            await SetCurrentUserInfoAsync();
            await UserConfigurationManager.GetAsync();

            //if (AppContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
            //{
            //    _navigationService.NavigateTo(NavigationUrlConsts.InforNTD);

            //}
            //else if (AppContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type2)
            //{
            //    _navigationService.NavigateTo(NavigationUrlConsts.InforNLD);

            //}
            //else
            //{
                _navigationService.NavigateTo(NavigationUrlConsts.TrangChu);

            //}
        }

        private async Task SetCurrentUserInfoAsync()
        {
            await WebRequestExecuter.Execute(async () =>
                await _sessionAppService.GetCurrentLoginInformations(), GetCurrentUserInfoExecuted);
        }

        private async Task GetCurrentUserInfoExecuted(GetCurrentLoginInformationsOutput result)
        {
            _applicationContext.SetLoginInfo(result);

            await _dataStorageService.StoreLoginInformationAsync(_applicationContext.LoginInfo);
        }
    }
}