using Abp.Application.Navigation;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Profile.Views.Shared.Components.ProfileSideBar
{
    public class UserTypeNavigationViewModel
    {
        public UserMenu AppMenu { get; set; }
        public UserMenu MegaMenu { get; set; }

        public MenuDefinition TopMenu { get; set; }
        public MenuDefinition FooterMenu { get; set; }

        public List<UserMenu> Menus { get; set; }
    }
}


