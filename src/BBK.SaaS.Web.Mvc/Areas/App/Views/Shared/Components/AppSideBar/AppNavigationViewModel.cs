using Abp.Application.Navigation;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.App.Views.Shared.Components.AppSideBar
{
    public class AppNavigationViewModel
    {
        public UserMenu MainMenu { get; set; }
        public UserMenu AppMenu { get; set; }
        public UserMenu MegaMenu { get; set; }

        public List<UserMenu> Menus { get; set; }
    }
}


