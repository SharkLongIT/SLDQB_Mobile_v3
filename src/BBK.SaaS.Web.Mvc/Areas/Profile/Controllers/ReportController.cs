using Abp.Authorization;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BBK.SaaS.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    [AbpAuthorize]
    public class ReportController : SaaSControllerBase
    {
        public IActionResult Index()
        {
            var Year = DateTime.Now.Year;
            var Month = DateTime.Now.Month;

            ViewBag.Year = Year;
            ViewBag.Month = Month;

            return View();
        }
        public IActionResult ReportCat()
        {
         

            return View();
        }
        public IActionResult ReportArticle()
        {
            return View();
        }

        public IActionResult ReportCatApex()
        {
            return View();
        }
        public IActionResult ReportArticleApex()
        {
            return View();
        }
    }
}
