using Abp.Dependency;
using Abp.Extensions;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Localization;
using BBK.SaaS.Models.NavigationMenu;
using BBK.SaaS.Services.Account;
using BBK.SaaS.Services.Permission;

namespace BBK.SaaS.Services.Navigation
{
    public class MenuProvider : ISingletonDependency, IMenuProvider
    {
        protected INavigationService navigationService { get; set; }
        protected IAccountService AccountService { get; set; }

        protected IApplicationContext ApplicationContext { get; set; }
        private bool IsUserLoggedIn;
        private bool IsDefault1;

        public MenuProvider()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            AccountService = DependencyResolver.Resolve<IAccountService>();

            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
        }
        /* For more icons:
            https://material.io/icons/
        */

        private List<NavigationMenuItem> _menuItems;
        public void InitializeMenuItems()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();
            if (IsUserLoggedIn)
            {
                #region Nhà tuyển dụng
                if (ApplicationContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                {
                    _menuItems = new List<NavigationMenuItem>
            {
               
                  //---------------------------------------------
                new NavigationMenuItem
                {
                    Title = L.Localize("Trang chủ"),
                    Icon = "fa fa-home",
                    NavigationUrl  = NavigationUrlConsts.TrangChu
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("Việc tìm người"),
                    Icon = "fas fa-briefcase",
                    NavigationUrl  = NavigationUrlConsts.ViecTimNguoi
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("Người tìm việc"),
                    Icon = "fa fa-user",
                    NavigationUrl  = NavigationUrlConsts.NguoiTimViec
                },

                //-Chuyên mục-------------------------------------------
                 new NavigationMenuItem
                 {
                    Title = L.Localize("Chuyên mục"),
                    Items = new List<NavigationMenuItem>
                    {
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Tin tức"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.TinTuc
                        },
                        // new NavigationMenuItem
                        //{
                        //    Title = L.Localize("detail Tin tức"),
                        //    //Icon = "fa fa-heart",
                        //    NavigationUrl  = NavigationUrlConsts.ArticleDetail
                        //},
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Thông tin du học"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.ThongTinDuHoc
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Thông tin xuất khẩu lao động"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.ThongTinXuatKhauLaoDong
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Đào tạo kĩ năng"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DaotaoKyNang
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Đào tạo nghề"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DaoTaoNghe
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Dịch vụ khác"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DichVuKhac
                        },
                          new NavigationMenuItem
                        {
                            Title = L.Localize("Văn bản mới"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.VanBanMoi
                        },
                    }


                },

                //--lien he---
                 new NavigationMenuItem
                 {
                        Title = L.Localize("Liên hệ & Hỏi đáp"),
                        Icon = "fa fa-phone",
                        NavigationUrl  = NavigationUrlConsts.LienHe
                 },
                 //  new NavigationMenuItem
                 //{
                 //       Title = L.Localize("Phiên giao dịch"),
                 //       Icon = "fab fa-ideal",
                 //       NavigationUrl  = NavigationUrlConsts.PhienGiaoDich
                 //},
                 new NavigationMenuItem
                 {
                        Title = L.Localize("Giới thiệu"),
                        Icon = "fa fa-info-circle",
                        NavigationUrl  = NavigationUrlConsts.GioiThieu
                 },


                 new NavigationMenuItem
                 {
                    Title = L.Localize("Tenants"),
                    Icon = "fa-solid fa-list",
                    NavigationUrl = NavigationUrlConsts.Tenants,
                    RequiredPermissionName = AppPermissions.Pages_Tenants,
                 },
                 new NavigationMenuItem
                 {
                    Title = L.Localize("Users"),
                    Icon = "fa-solid fa-filter",
                    NavigationUrl= NavigationUrlConsts.User,
                    RequiredPermissionName = AppPermissions.Pages_Administration_Users,
                 },


                 new NavigationMenuItem
                        {
                                Title = L.Localize("Thông tin cá nhân "),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.InforNTD,
                        },
                 new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách tin tuyển dụng của tôi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.BaiTuyenDung,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                 new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách ứng viên đã ứng tuyển"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DanhSachUVDUT,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                 new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách giới thiệu của tôi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DSGioiThieu,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                 new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách lịch hẹn"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DanhSachLichHen,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                 new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách câu hỏi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DSCauHoi,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                 new NavigationMenuItem
                        {
                            Title = L.Localize("Cài đặt"),
                            //Icon = "fa-solid fa-cog",
                            NavigationUrl  = NavigationUrlConsts.Settings,
                        },



                #endregion 

                        // new NavigationMenuItem
                        //{
                        //       Title = L.Localize("Đăng xuất"),
                        //       Icon = "fa fa-sign-out",

                        //       NavigationUrl  = NavigationUrlConsts.TrangChu

                        //},



                        /*This is a sample menu item to guide how to add a new item.
                            ,new NavigationMenuItem
                            {
                                Title = "Sample View",
                                Icon = "MyIcon.png",
                                TargetType = typeof(_SampleView),
                                Order = 10
                            }
                        */
                    };
                }
                else
                {

                    #region Người lao động
                    _menuItems = new List<NavigationMenuItem>
            {
               
                  //---------------------------------------------
                //new NavigationMenuItem
                //{
                //    Title = L.Localize("Trang chủ"),
                //    Icon = "fa fa-home",
                //    NavigationUrl  = NavigationUrlConsts.TrangChu
                //},
                //new NavigationMenuItem
                //{
                //    Title = L.Localize("Việc tìm người"),
                //    Icon = "fas fa-briefcase",
                //    NavigationUrl  = NavigationUrlConsts.ViecTimNguoi
                //},
                //new NavigationMenuItem
                //{
                //    Title = L.Localize("Người tìm việc"),
                //    Icon = "fa fa-user",
                //    NavigationUrl  = NavigationUrlConsts.NguoiTimViec
                //},

                ////-Chuyên mục-------------------------------------------
                // new NavigationMenuItem
                // {
                //    Title = L.Localize("Chuyên mục"),
                //    Items = new List<NavigationMenuItem>
                //    {
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Tin tức"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.TinTuc
                //        },
                //        // new NavigationMenuItem
                //        //{
                //        //    Title = L.Localize("detail Tin tức"),
                //        //    //Icon = "fa fa-heart",
                //        //    NavigationUrl  = NavigationUrlConsts.ArticleDetail
                //        //},
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Thông tin du học"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.ThongTinDuHoc
                //        },
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Thông tin xuất khẩu lao động"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.ThongTinXuatKhauLaoDong
                //        },
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Đào tạo kĩ năng"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.DaotaoKyNang
                //        },
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Đào tạo nghề"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.DaoTaoNghe
                //        },
                //        new NavigationMenuItem
                //        {
                //            Title = L.Localize("Dịch vụ khác"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.DichVuKhac
                //        },
                //          new NavigationMenuItem
                //        {
                //            Title = L.Localize("Văn bản mới"),
                //            //Icon = "fa fa-heart",
                //            NavigationUrl  = NavigationUrlConsts.VanBanMoi
                //        },
                //    }


                //},

                ////--lien he---
                // new NavigationMenuItem
                // {
                //        Title = L.Localize("Liên hệ & Hỏi đáp"),
                //        Icon = "fa fa-phone",
                //        NavigationUrl  = NavigationUrlConsts.LienHe
                // },
                // //  new NavigationMenuItem
                // //{
                // //       Title = L.Localize("Phiên giao dịch"),
                // //       Icon = "fab fa-ideal",
                // //       NavigationUrl  = NavigationUrlConsts.PhienGiaoDich
                // //},
                // new NavigationMenuItem
                // {
                //        Title = L.Localize("Giới thiệu"),
                //        Icon = "fa fa-info-circle",
                //        NavigationUrl  = NavigationUrlConsts.GioiThieu
                // },


                // new NavigationMenuItem
                // {
                //    Title = L.Localize("Tenants"),
                //    Icon = "fa-solid fa-list",
                //    NavigationUrl = NavigationUrlConsts.Tenants,
                //    RequiredPermissionName = AppPermissions.Pages_Tenants,
                // },
                // new NavigationMenuItem
                // {
                //    Title = L.Localize("Users"),
                //    Icon = "fa-solid fa-filter",
                //    NavigationUrl= NavigationUrlConsts.User,
                //    RequiredPermissionName = AppPermissions.Pages_Administration_Users,
                // },

                 new NavigationMenuItem
                 {
                    Title = L.Localize("Hồ sơ cá nhân"),
                    Items = new List<NavigationMenuItem>
                    {
                        new NavigationMenuItem
                        {
                                Title = L.Localize("Thông tin cá nhân "),
                                //Icon = "fa fa-user-circle",
                                NavigationUrl  = NavigationUrlConsts.InforNLD,
                        },
                        new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách hồ sơ ứng tuyển của tôi"),
                                //Icon = "fa fa-list-alt",
                                NavigationUrl  = NavigationUrlConsts.BaiUngTuyen,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                         new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách công việc đã ứng tuyển của tôi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DSCVDaUngTuyen,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                              new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách giới thiệu của tôi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DSGT,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                          new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách lịch hẹn"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DanhSachLH,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                         new NavigationMenuItem
                        {
                                Title = L.Localize("Danh sách câu hỏi của tôi"),
                                //Icon = "fa-solid fa-cog",
                                NavigationUrl  = NavigationUrlConsts.DanhSachCauHoi,
                                 //RequiredPermissionName = AppPermissions.,
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Cài đặt"),
                            //Icon = "fa-solid fa-cog",
                            NavigationUrl  = NavigationUrlConsts.Settings,
                        },
                    }
 }




                 // new NavigationMenuItem
                 //{
                 //       Title = L.Localize("Đăng xuất"),
                 //       Icon = "fa fa-sign-out",

                 //       //NavigationUrl  = NavigationUrlConsts.TrangChu,
                       
                 //       ClickAction = LogOutAndNavToHome,
                 //},
                   
           
           
                /*This is a sample menu item to guide how to add a new item.
                    ,new NavigationMenuItem
                    {
                        Title = "Sample View",
                        Icon = "MyIcon.png",
                        TargetType = typeof(_SampleView),
                        Order = 10
                    }
                */
            };
                    #endregion
                }

            }
            else
            {
                _menuItems = new List<NavigationMenuItem>
            {
               
                  //---------------------------------------------
                new NavigationMenuItem
                {
                    Title = L.Localize("Trang chủ"),
                    Icon = "fa fa-home",
                    NavigationUrl  = NavigationUrlConsts.TrangChu
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("Việc tìm người"),
                    Icon = "fas fa-briefcase",
                    NavigationUrl  = NavigationUrlConsts.ViecTimNguoi
                },
                new NavigationMenuItem
                {
                    Title = L.Localize("Người tìm việc"),
                    Icon = "fa fa-user",
                    NavigationUrl  = NavigationUrlConsts.NguoiTimViec
                },

                //-Chuyên mục-------------------------------------------
                 new NavigationMenuItem
                 {
                    Title = L.Localize("Chuyên mục"),
                    Items = new List<NavigationMenuItem>
                    {
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Tin tức"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.TinTuc
                        },
                        // new NavigationMenuItem
                        //{
                        //    Title = L.Localize("detail Tin tức"),
                        //    //Icon = "fa fa-heart",
                        //    NavigationUrl  = NavigationUrlConsts.ArticleDetail
                        //},
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Thông tin du học"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.ThongTinDuHoc
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Thông tin xuất khẩu lao động"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.ThongTinXuatKhauLaoDong
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Đào tạo kĩ năng"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DaotaoKyNang
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Đào tạo nghề"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DaoTaoNghe
                        },
                        new NavigationMenuItem
                        {
                            Title = L.Localize("Dịch vụ khác"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.DichVuKhac
                        },
                          new NavigationMenuItem
                        {
                            Title = L.Localize("Văn bản mới"),
                            //Icon = "fa fa-heart",
                            NavigationUrl  = NavigationUrlConsts.VanBanMoi
                        },
                    }


                },

                //--lien he---
                 new NavigationMenuItem
                 {
                        Title = L.Localize("Liên hệ & Hỏi đáp"),
                        Icon = "fa fa-phone",
                        NavigationUrl  = NavigationUrlConsts.LienHe
                 },
                 //  new NavigationMenuItem
                 //{
                 //       Title = L.Localize("Phiên giao dịch"),
                 //       Icon = "fab fa-ideal",
                 //       NavigationUrl  = NavigationUrlConsts.PhienGiaoDich
                 //},
                 new NavigationMenuItem
                 {
                        Title = L.Localize("Giới thiệu"),
                        Icon = "fa fa-info-circle",
                        NavigationUrl  = NavigationUrlConsts.GioiThieu
                 },


                 new NavigationMenuItem
                 {
                    Title = L.Localize("Tenants"),
                    Icon = "fa-solid fa-list",
                    NavigationUrl = NavigationUrlConsts.Tenants,
                    RequiredPermissionName = AppPermissions.Pages_Tenants,
                 },
                 new NavigationMenuItem
                 {
                    Title = L.Localize("Users"),
                    Icon = "fa-solid fa-filter",
                    NavigationUrl= NavigationUrlConsts.User,
                    RequiredPermissionName = AppPermissions.Pages_Administration_Users,
                 },
                  new NavigationMenuItem
                 {
                        Title = L.Localize("Đăng nhập"),
                        Icon = "fa fa-sign-in",
                        NavigationUrl  = NavigationUrlConsts.Login


                 },
                   
           
           
                /*This is a sample menu item to guide how to add a new item.
                    ,new NavigationMenuItem
                    {
                        Title = "Sample View",
                        Icon = "MyIcon.png",
                        TargetType = typeof(_SampleView),
                        Order = 10
                    }
                */
            };

            }

        }

        //private void LogOutAndNavToHome()
        //{
        //     AccountService.LogoutAsync();
        //    //navigationService.NavigateTo("/", forceLoad: true);
        //    AccountService.AbpAuthenticateModel.UserNameOrEmailAddress = null;

        //    navigationService.NavigateTo(NavigationUrlConsts.TrangChu);
        //}
        public List<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions)
        {
            InitializeMenuItems();
            return FilterAuthorizedMenuItems(_menuItems, grantedPermissions);
        }

        private List<NavigationMenuItem> FilterAuthorizedMenuItems(List<NavigationMenuItem> menuItems, Dictionary<string, string> grantedPermissions)
        {
            var authorizedMenuItems = new List<NavigationMenuItem>();
            foreach (var menuItem in menuItems)
            {
                var authorizedMenuItem = new NavigationMenuItem()
                {
                    Title = menuItem.Title,
                    Icon = menuItem.Icon,
                    IsSelected = menuItem.IsSelected,
                    NavigationParameter = menuItem.NavigationParameter,
                    NavigationUrl = menuItem.NavigationUrl,
                    RequiredPermissionName = menuItem.RequiredPermissionName
                };

                if (menuItem.Items.Any())
                {
                    var authorizedSubMenuItems = FilterAuthorizedMenuItems(menuItem.Items, grantedPermissions);
                    if (authorizedMenuItem.NavigationUrl.IsNullOrEmpty() && !authorizedSubMenuItems.Any())
                    {
                        continue;
                    }

                    authorizedMenuItem.Items.AddRange(authorizedSubMenuItems);
                }

                if (menuItem.RequiredPermissionName == null)
                {
                    authorizedMenuItems.Add(authorizedMenuItem);
                    continue;
                }

                if (grantedPermissions != null &&
                    grantedPermissions.ContainsKey(menuItem.RequiredPermissionName))
                {
                    authorizedMenuItems.Add(authorizedMenuItem);
                }
            }

            return authorizedMenuItems;
        }
    }
}