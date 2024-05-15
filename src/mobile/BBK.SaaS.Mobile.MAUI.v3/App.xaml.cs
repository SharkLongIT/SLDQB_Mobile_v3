using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Services.Account;
using BBK.SaaS.Services.Storage;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace BBK.SaaS.Mobile.MAUI
{
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage();

            App.Current.On<Microsoft.Maui.Controls.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

        }

        public static async Task OnSessionTimeout()
        {
            await DependencyResolver.Resolve<IAccountService>().LogoutAsync();
        }

        public static async Task OnAccessTokenRefresh(string newAccessToken, string newEncryptedAccessToken)
        {
            await DependencyResolver.Resolve<IDataStorageService>().StoreAccessTokenAsync(newAccessToken, newEncryptedAccessToken);
        }

        public static void LoadPersistedSession()
        {
            var accessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();
            var dataStorageService = DependencyResolver.Resolve<IDataStorageService>();
            var applicationContext = DependencyResolver.Resolve<IApplicationContext>();

            accessTokenManager.AuthenticateResult = dataStorageService.RetrieveAuthenticateResult();
            applicationContext.Load(dataStorageService.RetrieveTenantInfo(), dataStorageService.RetrieveLoginInfo());
        }
    }
}