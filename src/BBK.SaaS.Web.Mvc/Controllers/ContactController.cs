using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Controllers
{
    public class ContactController : SaaSControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
