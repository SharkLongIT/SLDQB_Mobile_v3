using BBK.SaaS.Models.NavigationMenu;

namespace BBK.SaaS.Services.Navigation
{
    public interface IMenuProvider
    {
        List<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}