using Abp.Application.Navigation;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;
using BBK.SaaS.Configuration;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Views.Shared.Components.CmsMenus
{
    public class CmsMenusViewComponent : SaaSViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;
        private readonly IConfiguration _configuration;
        private readonly ILocalizationContext _localizationContext;
        private readonly ICmsCatsAppService _catsAppService;
        private readonly IFECntCategoryAppService _feContentCtgAppService;

        public CmsMenusViewComponent(
            IConfiguration configuration,
            ILocalizationContext localizationContext,
            IUserNavigationManager userNavigationManager,
            ICmsCatsAppService catsAppService,
            IFECntCategoryAppService feContentCtgAppService,
            IAbpSession abpSession)
        {
            _configuration = configuration;
            _localizationContext = localizationContext;
            _userNavigationManager = userNavigationManager;
            _catsAppService = catsAppService;
            _feContentCtgAppService = feContentCtgAppService;
            _abpSession = abpSession;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var layoutType = await SettingManager.GetSettingValueAsync(AppSettings.UserManagement.AllowSelfRegistration);

            var catMenus = await _feContentCtgAppService.GetMenuCategories();

            var menuDef = GetDefaultCmsMenu();

            foreach (var catMenu in catMenus.Items.Where(x => x.ParentId == null).ToList())
            {
                var currentCatMenu = menuDef.AddItem(new MenuItemDefinition(
                        $"Home.ContentManagement{catMenu.Code}",
                        new FixedLocalizableString($"{catMenu.DisplayName}"),
                        url: $"/{catMenu.Slug}"
                    ));

                foreach (var child in catMenus.Items.Where(x => x.ParentId == catMenu.Id).ToList())
                {
                    currentCatMenu.AddItem(new MenuItemDefinition(
                        $"Home.ContentManagement{child.Code}",
                        new FixedLocalizableString($"{child.DisplayName}"),
                        url: $"/{child.Slug}"
                    ));
                }
            }

            //_configuration.Get
            var model = new CmsMenusViewModel
            {
                //MainMenu = await _userNavigationManager.GetMenuAsync("MainMenu", _abpSession.ToUserIdentifier())
                //TopMenu = AsyncHelper.RunSync(() => (GetDefaultCmsMenu()),
                TopMenu = menuDef,
                //FooterMenu = await _userNavigationManager.GetMenuAsync(AppNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
                //MegaMenu = await _userNavigationManager.GetMenuAsync(ProfileNavigationProvider.MenuName, _abpSession.ToUserIdentifier()),
            };
            //model.Menus = new System.Collections.Generic.List<UserMenu>();

            //foreach (var menu in await _userNavigationManager.GetMenusAsync(_abpSession.ToUserIdentifier()))
            //{
            //	if (menu.Name != AppNavigationProvider.MenuName && menu.Name != "MainMenu")
            //	{
            //		model.Menus.Add(menu);
            //	}
            //}
            //_navCtx.Manager.MainMenu

            return View(model);
        }

        /// <summary>
        /// Demo Menu: Should be add into .json of tenant or in dbtable
        /// </summary>
        /// <returns></returns>
        public MenuDefinition GetDefaultCmsMenu()
        {
            var userMenu = new UserMenu();
            var menu = new MenuDefinition("Home", new FixedLocalizableString("Home Menu"));

            menu
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Giới thiệu"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Giới thiệu về Quảng Bình"),
                            url: "/",
                            icon: "flaticon-users"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Giới thiệu về Trung tâm"),
                            url: "/",
                            icon: "flaticon-users"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Chức năng nhiệm vụ của Trung tâm"),
                            url: "/",
                            icon: "flaticon-users"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Cơ cấu tổ chức"),
                            url: "/",
                            icon: "flaticon-users"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    )
                )
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Người tìm việc"),
                        url: "/UserJob")
                )
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Việc tìm người"),
                        url: "/JobUser/Search")
                )
                  .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Liên hệ hỏi đáp"),
                        url: "/Contact")
                )
                ;

            return menu;
        }
    }
}
