using Abp.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Controllers;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
    [Area("App")]
    [AbpMvcAuthorize]
    public class WelcomeController : SaaSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}