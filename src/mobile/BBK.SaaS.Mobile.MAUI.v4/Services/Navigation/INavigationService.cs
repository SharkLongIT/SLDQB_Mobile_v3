using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Services.Navigation
{
    public interface INavigationService
    {
        void Initialize(NavigationManager navigationManager);
       
        bool IsUserLoggedIn(); // check Login
        void NavigateTo(string uri, bool forceLoad = false, bool replace = false);
    }
}