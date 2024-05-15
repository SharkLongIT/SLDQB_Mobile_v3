using Abp.Application.Navigation;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Views.Shared.Components.CmsMenus
{
    public class CmsMenusViewModel
    {
        public MenuDefinition TopMenu { get; set; }
        public MenuDefinition FooterMenu { get; set; }
        public UserMenu MegaMenu { get; set; }

        public List<MenuDefinition> Menus { get; set; }
    }
}


