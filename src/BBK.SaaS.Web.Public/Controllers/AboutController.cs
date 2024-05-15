using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Web.Controllers;

namespace BBK.SaaS.Web.Public.Controllers
{
    public class AboutController : SaaSControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}