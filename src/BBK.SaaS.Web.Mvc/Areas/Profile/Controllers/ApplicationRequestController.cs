using Abp.AspNetCore.Mvc.Authorization;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Web.Areas.Profile.Models.ApplicationRequestModel;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    public class ApplicationRequestController : SaaSControllerBase
    {
        private readonly IApplicationRequestAppService _appRequest;
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        public ApplicationRequestController(IApplicationRequestAppService appRequest,
          IJobApplicationAppService jobApplicationAppService, ICatUnitAppService catUnitAppService)
        {
            _appRequest = appRequest;
            _jobApplicationAppService = jobApplicationAppService;
            _catUnitAppService = catUnitAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CreateApplicationRequestModal(long RecruitmentId)
        {
            ViewBag.RecruitmentId = RecruitmentId;
            var JobApplications = await _jobApplicationAppService.GetListJobAppOfCandidate(new Mdls.Profile.Candidates.Dto.JobAppSearch());
            List<SelectListItem> ListJobAppOrCandidate = new List<SelectListItem>();
            ListJobAppOrCandidate.AddRange(JobApplications.Items.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString(),
            }));
            ViewBag.ListJobAppOrCandidate = ListJobAppOrCandidate;

            return PartialView("CreateApplicationRequestModal");
        }
        
        public async Task<IActionResult> ApplicationRequestModal(long Id) 
        {
            ApplicationRequestModel model = new ApplicationRequestModel();

            var JobApplicationRequest = await _appRequest.GetById(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = Id});
            model = ObjectMapper.Map< ApplicationRequestModel>(JobApplicationRequest);
            return PartialView("ApplicationRequestModal", model );
        }

        public async Task<JsonResult> Create(ApplicationRequestModel input)
        {
            ApplicationRequestEditDto applicationRequest = new Mdls.Profile.ApplicationRequests.Dto.ApplicationRequestEditDto();
            if (input == null)
            {
                applicationRequest.RecruitmentId = input.RecruitmentId;
                applicationRequest.JobApplicationId = input.JobApplicationId;
            }
            var AppRequest = _appRequest.Create(applicationRequest);
            return Json(AppRequest);
        }

        public IActionResult ApplicationRequestByRecruiter()
        {
            #region kinh nghiệm
            var CatUnitEX = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnitEX != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitEX.Id).Result;
                List<SelectListItem> listItemsKinhNghiem = new List<SelectListItem>();
                listItemsKinhNghiem.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKinhNghiem = listItemsKinhNghiem;

            }
            #endregion

            #region cấp bậc
            var CatUnitRank = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
            if (CatUnitRank != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id).Result;
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion
            return View();
        }

        [AbpMvcAuthorize]
        public async Task<long> Delete(long Id)
        {
            try
            {
                await _appRequest.Delete(Id);
                return Id;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}
