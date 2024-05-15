using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Session;

namespace BBK.SaaS.Web.Views.Shared.Components.AccountLogo
{
    public class AccountLogoViewComponent : SaaSViewComponent
    {
        private readonly IPerRequestSessionCache _sessionCache;

        public AccountLogoViewComponent(IPerRequestSessionCache sessionCache)
        {
            _sessionCache = sessionCache;
        }

        public async Task<IViewComponentResult> InvokeAsync(string skin)
        {
            var loginInfo = await _sessionCache.GetCurrentLoginInformationsAsync();
            return View(new AccountLogoViewModel(loginInfo, skin));
        }
    }
}
