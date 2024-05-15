using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.HtmlSanitizer;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.Contacts;

namespace BBK.SaaS.Web.Controllers
{
    public class ContactController : SaaSControllerBase
    {
        private readonly IContactAppService _contactAppService;

        public ContactController(IContactAppService contactAppService)
        {
            _contactAppService = contactAppService;
        }        //[AbpAuthorize]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Create(ContactDto model)
        {
            try
            {
                var tradingaccount = await _contactAppService.Create(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
    }
}
