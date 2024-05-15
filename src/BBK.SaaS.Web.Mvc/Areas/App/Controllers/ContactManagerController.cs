using Abp.AspNetCore.Mvc.Authorization;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
    [Area("App")]
    [AbpMvcAuthorize]
    public class ContactManagerController : SaaSControllerBase
    {
        private readonly IContactAppService _contactAppService;
        public ContactManagerController(IContactAppService contactAppService)
        {
            _contactAppService = contactAppService;
        }

        public IActionResult Index()
        {
            return View();
        }


        public IActionResult ContactDetail(long Id)
        {
            ContactSearch contactSearch = new ContactSearch();
            var info = _contactAppService.GetAll(contactSearch).Result.Items.Where(x => x.Id == Id).FirstOrDefault();
            ContactDto contactDto = new ContactDto();
            contactDto.Id = Id;
            contactDto.Status = info.Status;
            contactDto.Email = info.Email;
            contactDto.FullName = info.FullName;
            contactDto.Phone = info.Phone;
            contactDto.CreationTime = info.CreationTime;
            contactDto.Description = info.Description;
            contactDto.Answer = info.Answer;

            return PartialView(contactDto);
        }
    }
}
