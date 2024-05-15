using Abp.Authorization;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    [AbpAuthorize]
    public class IntroduceController : SaaSControllerBase
    {
        private readonly IIntroduceAppService _introduceAppService;
        public IntroduceController(IIntroduceAppService introduceAppService)
        {
            _introduceAppService = introduceAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ViewIntroduce(long Id)
        {
            IntroduceSearch introduceSearch = new IntroduceSearch();
            var InFo =  _introduceAppService.GetAll(introduceSearch).Result.Items.Where(x=>x.Id == Id).FirstOrDefault();
            var model = new IntroduceEditDto();
            model.Id = Id;
            model.FullName = InFo.FullName;
            model.Email = InFo.Email;
            model.Phone = InFo.Phone;
            model.Description = InFo.Description;
            model.Status = InFo.Status;
            model.ArticleId = InFo.ArticleId;

            return PartialView("ViewIntroduce", model);
        } 
        
        public async Task<IActionResult> Detail(long Id)
        {
            IntroduceSearch introduceSearch = new IntroduceSearch();
            var InFo =  _introduceAppService.GetAll(introduceSearch).Result.Items.Where(x=>x.Id == Id).FirstOrDefault();
            var model = new IntroduceEditDto();
            model.Id = Id;
            model.FullName = InFo.FullName;
            model.Email = InFo.Email;
            model.Phone = InFo.Phone;
            model.Description = InFo.Description;
            model.Status = InFo.Status;
            model.ArticleId = InFo.ArticleId;

            return PartialView("Detail", model);
        }
    }
}
