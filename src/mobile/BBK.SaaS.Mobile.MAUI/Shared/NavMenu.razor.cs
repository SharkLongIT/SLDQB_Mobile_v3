using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mobile.MAUI.Services.UI;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Models.NavigationMenu;
using BBK.SaaS.Services.Account;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;

namespace BBK.SaaS.Mobile.MAUI.Shared
{
    public partial class NavMenu : SaaSComponentBase
    {
        private ItemsProviderResult<CmsCatDto> cmsCatDto;
        private Virtualize<CmsCatDto> CmsContainer { get; set; }
        private readonly GetArticlesByCatInput _filter = new GetArticlesByCatInput();
        protected ICmsCatsAppService cmsCatsAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IAccountService AccountService { get; set; }


        protected IMenuProvider MenuProvider { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IAccessTokenManager AccessTokenManager { get; set; }
        protected LanguageService LanguageService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }

        protected List<NavigationMenuItem> MenuItems;

        protected List<CmsCatDto> cmsCatDtos;

        private bool HasUserInfo => AccessTokenManager != null &&
            AccessTokenManager.IsUserLoggedIn &&
            ApplicationContext != null &&
            ApplicationContext.LoginInfo != null &&
            ApplicationContext?.LoginInfo?.User != null;

        private string _userImage;

        protected override async Task OnInitializedAsync()
        {
            MenuItems = new List<NavigationMenuItem>();

            MenuProvider = DependencyResolver.Resolve<IMenuProvider>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            AccessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            cmsCatsAppService = DependencyResolver.Resolve<ICmsCatsAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
            AccountService = DependencyResolver.Resolve<IAccountService>();
            LanguageService = DependencyResolver.Resolve<LanguageService>();
            LanguageService.OnLanguageChanged += (s, e) =>
            {
                BuildMenuItems();
                StateHasChanged();
            };

            BuildMenuItems();
            await GetUserPhoto();
            await GetCatDtosAsync();
            //await LoadCategories(new ItemsProviderRequest());
        }

        public void BuildMenuItems()
        {
            if (!AccessTokenManager.IsUserLoggedIn)
            {
                MenuItems = MenuProvider.GetAuthorizedMenuItems(null);
                return;
            }

            var grantedPermissions = ApplicationContext.Configuration.Auth.GrantedPermissions;
            MenuItems = MenuProvider.GetAuthorizedMenuItems(grantedPermissions);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await Task.Delay(1000);
                await JS.InvokeVoidAsync("KTDialer.init");
                await JS.InvokeVoidAsync("KTDrawer.init");
                await JS.InvokeVoidAsync("KTImageInput.init");
                await JS.InvokeVoidAsync("KTMenu.init");
                //await JS.InvokeVoidAsync("MenuSideBar");
                await JS.InvokeVoidAsync("KTPasswordMeter.init");
                await JS.InvokeVoidAsync("KTScroll.init");
                await JS.InvokeVoidAsync("KTScrolltop.init");
                await JS.InvokeVoidAsync("KTSticky.init");
                await JS.InvokeVoidAsync("KTSwapper.init");
                await JS.InvokeVoidAsync("KTToggle.init");
                await JS.InvokeVoidAsync("KTApp.init");
                await JS.InvokeVoidAsync("KTAppLayoutBuilder.init");
                await JS.InvokeVoidAsync("KTLayoutSearch.init");
                await JS.InvokeVoidAsync("KTAppSidebar.init");
                await JS.InvokeVoidAsync("KTThemeModeUser.init");
                await JS.InvokeVoidAsync("KTThemeMode.init");
                await JS.InvokeVoidAsync("KTLayoutToolbar.init");
            }
        }

        private async Task GetUserPhoto()
        {
            if (!HasUserInfo)
            {
                return;
            }

            _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
            StateHasChanged();
        }
        #region Category
        long CategoryCount;
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
                CategoryCount = articlesFilter.Count;
                cmsCatDto = new ItemsProviderResult<CmsCatDto>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
            }
        );

            return cmsCatDto;
        }

        private async Task<List<CmsCatDto>> GetCatDtosAsync()
        {
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await cmsCatsAppService.GetCmsCats(),
            async (result) =>
            {
                var articlesFilter = result.Items.ToList();
                cmsCatDtos = articlesFilter;
                await UserDialogsService.UnBlock();
            }
        );

            return cmsCatDtos;

        }
        #endregion

        public async Task GetArticleByCategory(CmsCatDto cmsCatDto)
        {
            navigationService.NavigateTo($"ListArticle?CategoryId={cmsCatDto.Id}&CategoryName={cmsCatDto.DisplayName}");
            //await LoadCategories(new ItemsProviderRequest());
            //StateHasChanged();
        }
        public async Task Logout()
        {

            var notLogout = await UserDialogsService.ConfirmLogout("Đăng xuất khỏi tài khoản của bạn?", "Đăng xuất");
            if (notLogout == false)
            {
                await AccountService.LogoutAsync();
                AccountService.AbpAuthenticateModel.UserNameOrEmailAddress = null;
                await UserDialogsService.AlertSuccess("Đăng xuất thành công");
                navigationService.NavigateTo(NavigationUrlConsts.TrangChu);
            }
            else
            {
            }

            StateHasChanged();


        }
    }
}
