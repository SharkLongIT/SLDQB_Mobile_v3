using Abp.Application.Navigation;
using Abp.Localization;
using Abp.Runtime.Session;
using BBK.SaaS.Configuration;
using BBK.SaaS.Web.Areas.App.Startup;
using BBK.SaaS.Web.Session;
using BBK.SaaS.Web.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Profile.Views.Shared.Components.ProfileSideBar
{
    public class ProfileSideBarViewComponent : SaaSViewComponent
    {
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly IAbpSession _abpSession;
        private readonly IConfiguration _configuration;
        //private readonly INavigationProviderContext _navCtx;
        private readonly IPerRequestSessionCache _sessionCache;

        public ProfileSideBarViewComponent(
            IConfiguration configuration,
            IUserNavigationManager userNavigationManager,
            //INavigationProviderContext navCtx,
            IPerRequestSessionCache sessionCache,
            IAbpSession abpSession)
        {
            _configuration = configuration;
            _userNavigationManager = userNavigationManager;
            //_navCtx = navCtx;
            _abpSession = abpSession;
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //var layoutType = await SettingManager.GetSettingValueAsync(AppSettings.UserManagement.AllowSelfRegistration);
            //Logger.Info($"MYLAYOUTIS:{layoutType}");
            var menuDef = new MenuDefinition("Default", new FixedLocalizableString("Default"));
            var LoginInformations = await _sessionCache.GetCurrentLoginInformationsAsync();

            if (LoginInformations.User.UserType == Authorization.Users.UserTypeEnum.Type1)
            {
                menuDef = GetDefaultNTDMenu();
            }
            else if (LoginInformations.User.UserType == Authorization.Users.UserTypeEnum.Type2)
            {
                menuDef = GetDefaultNLDMenu();

            }

            //_configuration.Get
            var model = new UserTypeNavigationViewModel
            {
                //MainMenu = await _userNavigationManager.GetMenuAsync("MainMenu", _abpSession.ToUserIdentifier())
                TopMenu = menuDef,
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
        public MenuDefinition GetDefaultNLDMenu()
        {
            var userMenu = new UserMenu();
            var menu = new MenuDefinition("Home", new FixedLocalizableString("Home Menu"));

            menu
                  .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Trang chủ"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#header-home",
                        url: "/Home/Index"

                    ))
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Quản lý tài khoản"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Thông tin cá nhân"),
                            url: "/Profile/Candidate/Detail",
                            icon: "fa-solid fa-user"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    )
                )
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Quản lý ứng tuyển")
                        )
                .AddItem(
                        new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách hồ sơ ứng tuyển"),
                        url: "/Profile/Candidate/JobAppOfCandidate")
                        ).AddItem(
                        new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Việc làm đã ứng tuyển"),
                        url: "/Profile/ApplicationRequest")
                        ).AddItem(
                        new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách lịch hẹn"),
                        url: "/Profile/Candidate/MakeAnAppointment")
                        ))
                .AddItem(
                        new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Liên kết giới thiệu"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                        ).AddItem(
                        new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách giới thiệu của tôi"),
                        url: "/Profile/Candidate/Introduce")
                        ))
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Liên hệ hỏi đáp"), 
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                        ).AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách câu hỏi của tôi"),
                        url: "/Profile/Candidate/QuestionsOfMe"
                        )))
               .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Phiên giao dịch"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách phiên đã tham gia"),
                        url: "/Profile/Candidate/TradingSessionsOfMe"
                        )));



            return menu;
        }

        /// <summary>
        /// Demo Menu: Should be add into .json of tenant or in dbtable
        /// </summary>
        /// <returns></returns>
        public MenuDefinition GetDefaultNTDMenu()
        {
            var userMenu = new UserMenu();
            var menu = new MenuDefinition("Home", new FixedLocalizableString("Home Menu"));

            menu
                 .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Trang chủ"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase",
                        url: "/Home/Index"

                    ))
                .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Quản lý tài khoản"),
                        icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"


                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                            new FixedLocalizableString("Thông tin nhà tuyển dụng"),
                            url: "/Profile/Recruiters/RecruiterInfo",
                            icon: "fa-solid fa-user"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                    )
                )




                .AddItem(new MenuItemDefinition(
                       "Home.ContentManagement",
                         new FixedLocalizableString("Quản lý tuyển dụng"),
                          icon: "/themes/mofi/assets/svg/icon-sprite.svg#stroke-knowledgebase"
                    ).AddItem(new MenuItemDefinition(
                            "Home.ContentManagement.Pages",
                             new FixedLocalizableString("Đăng tin tuyển dụng"),
                            url: "/Profile/Recruitments/Recruitment",
                            icon: "flaticon-users"
                        //permissionDependency: new SimplePermissionDependency(AppPermissions.Pages_Administration_Users)
                        )
                )
                     .AddItem(new MenuItemDefinition(
                            "Home.ContentManagement",
                            new FixedLocalizableString("Danh sách lịch hẹn"),
                            url: "/Profile/Appointments/Index"
                        )
                )
                      .AddItem(new MenuItemDefinition(
                         "Home.ContentManagement",
                         new FixedLocalizableString("Danh sách ứng tuyển"),
                         url: "/Profile/ApplicationRequest/ApplicationRequestByRecruiter"
                    )
                ).AddItem(new MenuItemDefinition(
                         "Home.ContentManagement",
                         new FixedLocalizableString("Danh sách giới thiệu của tôi"),
                         url: "/Profile/Recruiters/Introduce"
                    )
                ))

                   .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Liên hệ hỏi đáp")

                    ).AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách câu hỏi của tôi"),
                         url: "/Profile/Recruiters/QuestionsOfMe"
                        )))
                         .AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Phiên giao dịch")

                    ).AddItem(new MenuItemDefinition(
                        "Home.ContentManagement",
                        new FixedLocalizableString("Danh sách phiên đã tham gia"),
                         url: "/Profile/Recruiters/TradingSessionsOfMe"
                        ))

                    );
            //.AddItem(new MenuItemDefinition(
            //            "Home.ContentManagement",
            //            new FixedLocalizableString("Người tìm việc"),
            //           url: "/Profile/JobApplication/UserJob")
            //    )
            //.AddItem(new MenuItemDefinition(
            //            "Home.ContentManagement",
            //            new FixedLocalizableString("Việc tìm người"),
            //            url: "/Profile/Recruitments/JobUser")
            //        );

            return menu;
        }
    }
}
