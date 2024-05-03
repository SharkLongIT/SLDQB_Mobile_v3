using Flurl.Http;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using Plugin.Connectivity;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using BBK.SaaS.Mobile.MAUI.Services.Account;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using Microsoft.JSInterop;
using BBK.SaaS.Mobile.MAUI.Services.Tenants;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Services.Account;








#if ANDROID
using BBK.SaaS.Mobile.MAUI.Platforms.Android.HttpClient;
#endif

namespace BBK.SaaS.Mobile.MAUI.Shared.Layout
{
    public partial class MainLayout
    {

        private ItemsProviderResult<CmsCatDto> cmsCatDto;
        private Virtualize<CmsCatDto> CmsContainer { get; set; }
        private readonly GetArticlesByCatInput _filter = new GetArticlesByCatInput();
        protected ICmsCatsAppService cmsCatsAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IAccountService AccountService { get; set; }
        protected IAccessTokenManager AccessTokenManager { get; set; }

        protected UserProfileService userProfileService { get; set; }   

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IJSRuntime JS { get; set; }

        protected UserDialogsService UserDialogsService { get; set; }

        private bool IsConfigurationsInitialized { get; set; }


        private bool HasUserInfo => AccessTokenManager != null &&
          AccessTokenManager.IsUserLoggedIn &&
          ApplicationContext != null &&
          ApplicationContext.LoginInfo != null &&
          ApplicationContext?.LoginInfo?.User != null;

        private string _logoURL;
        private string _userImage;

        public MainLayout()
        {
            cmsCatsAppService = DependencyResolver.Resolve<ICmsCatsAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            userProfileService = DependencyResolver.Resolve<UserProfileService>();
            AccountService = DependencyResolver.Resolve<IAccountService>();
            AccessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();

        }

        protected override async Task OnInitializedAsync()
        {
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();
            UserDialogsService.Initialize(JS);

            await UserDialogsService.Block();

            await CheckInternetAndStartApplication();

            var navigationService = DependencyResolver.Resolve<INavigationService>();
            navigationService.Initialize(NavigationManager);
            await SetLayout();
            await GetUserPhoto();
        }
        private async Task GetUserPhoto()
        {
            if (!HasUserInfo)
            {
                return;
            }

            _userImage = await userProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
            StateHasChanged();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(200);
                await JS.InvokeVoidAsync("KTMenu.init");
            }
        }

        private async Task CheckInternetAndStartApplication()
        {
            if (CrossConnectivity.Current.IsConnected || ApiUrlConfig.IsLocal)
            {
                await StartApplication();
            }
            else
            {
                var isTryAgain = await UserDialogsService.Instance.Confirm(Localization.L.Localize("Không có kết nối mạng"));
                if (!isTryAgain)
                {
                    Application.Current.Quit();
                }

                await CheckInternetAndStartApplication();
            }
        }

        private async Task StartApplication()
        {
            /*
              If you are using Genymotion Emulator, set DebugServerIpAddresses.Current = "10.0.3.2".
              If you are using a real Android device, set it as your computer's local IP and 
                 make sure your Android device and your computer is connecting to the internet via your local Wi-Fi.
           */
            DebugServerIpAddresses.Current = "10.0.2.2";

            ConfigureFlurlHttp();
            App.LoadPersistedSession();

            if (UserConfigurationManager.HasConfiguration)
            {
                IsConfigurationsInitialized = true;
                await UserDialogsService.UnBlock();

            }
            else
            {
                await UserConfigurationManager.GetAsync(async () =>
                {
                    IsConfigurationsInitialized = true;
                    await UserDialogsService.UnBlock();
                });
            }
        }

        private static void ConfigureFlurlHttp()
        {
#if ANDROID
            var abpHttpClientFactory = new AndroidHttpClientFactory
            {
                OnSessionTimeOut = App.OnSessionTimeout,
                OnAccessTokenRefresh = App.OnAccessTokenRefresh
            };
#elif IOS
            var abpHttpClientFactory = new BBK.SaaS.Mobile.MAUI.Platforms.iOS.iOSHttpClientFactory
            {
                OnSessionTimeOut = App.OnSessionTimeout,
                OnAccessTokenRefresh = App.OnAccessTokenRefresh
            };
#endif
            FlurlHttp.Configure(c =>
            {
                c.HttpClientFactory = abpHttpClientFactory;
            });
        }

        private async Task SetLayout()
        {
            var dom = DependencyResolver.Resolve<DomManipulatorService>();
            await dom.ClearAllAttributes(JS, "body");
            //await dom.SetAttribute(JS, "body", "id", "kt_app_body");
            //await dom.SetAttribute(JS, "body", "data-kt-app-layout", "light-sidebar");
            //await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-enabled", "true");
            //await dom.SetAttribute(JS, "body", "data-kt-app-sidebar-fixed", "true");
            //await dom.SetAttribute(JS, "body", "data-kt-app-toolbar-enabled", "true");
            //await dom.SetAttribute(JS, "body", "class", "app-default");
            await dom.SetAttribute(JS, "body", "class", "theme-light");
            await dom.SetAttribute(JS, "body", "data-highlight", "red");
            await dom.SetAttribute(JS, "body", "data-gradient", "body-default");
        }


        #region open side bar
        bool IsOpenSideBar;
        public async Task OpenSideBar()
        {
            IsOpenSideBar = true;
            await JS.InvokeVoidAsync("menuSideBar");
        }
        public async Task CloseSideBar()
        {
            IsOpenSideBar = false;
        }
        #endregion
        #region Category

        private async ValueTask<ItemsProviderResult<CmsCatDto>> LoadCategories(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await cmsCatsAppService.GetCmsCats(),
            async (result) =>
            {
                var articlesFilter = result.Items.ToList();
                cmsCatDto = new ItemsProviderResult<CmsCatDto>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
            }
        );

            return cmsCatDto;
        }
        #endregion


        public async Task GetArticleByCategory(CmsCatDto cmsCatDto)
        {
            navigationService.NavigateTo($"ListArticle?CategoryId={cmsCatDto.Id}&CategoryName={cmsCatDto.DisplayName}");
        }
        public async Task Logout()
        {
            IsOpenSideBar = false;
            await AccountService.LogoutAsync();
            AccountService.AbpAuthenticateModel.UserNameOrEmailAddress = null;
            //await UserDialogsService.AlertSuccess("Đăng xuất thành công");
            navigationService.NavigateTo(NavigationUrlConsts.TrangChu);

            StateHasChanged();


        }
    }
}
