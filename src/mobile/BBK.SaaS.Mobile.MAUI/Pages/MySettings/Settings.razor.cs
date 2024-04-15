using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Account;
using BBK.SaaS.Services.Navigation;

namespace BBK.SaaS.Mobile.MAUI.Pages.MySettings
{
    public partial class Settings : SaaSMainLayoutPageComponentBase
    {
        protected IAccountService AccountService { get; set; }
        protected NavMenu NavMenu { get; set; }

        protected INavigationService navigationService { get; set; }
        ChangePasswordModal changePasswordModal;


       

        public Settings()
        {
            AccountService = DependencyResolver.Resolve<IAccountService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader("Cài đặt");          
        }
        // logout return TrangChu
        private async Task LogOut()
        {
            await AccountService.LogoutAsync();
            //navigationService.NavigateTo(NavigationUrlConsts.Login);

            navigationService.NavigateTo("/", forceLoad: true);
            AccountService.AbpAuthenticateModel.UserNameOrEmailAddress = null;
         
            navigationService.NavigateTo(NavigationUrlConsts.TrangChu);
        }

        private async Task OnChangePasswordAsync()
        {
            await changePasswordModal.Hide();
            await Task.Delay(300);
            await LogOut();
        }

        private async Task OnLanguageSwitchAsync()
        {
            await SetPageHeader(L("Cài đặt"));
            StateHasChanged();
        }

        private async Task ChangePassword()
        {
            await changePasswordModal.Show();
        }
      
    }
}
