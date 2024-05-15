using Abp.AspNetCore.Mvc.Authorization;
using Abp.HtmlSanitizer;
using Abp.Web.Models;
using AutoMapper.Internal;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
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
    [AbpMvcAuthorize]
    public class AppointmentsController : SaaSControllerBase
    {
        private readonly IMakeAnAppointmentAppService _makeAnAppointmentAppService;
        private readonly IRecruitmentAppService _recruitmentAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        public AppointmentsController(IMakeAnAppointmentAppService makeAnAppointmentAppService,
            IRecruitmentAppService recruitmentAppService,
            ICatUnitAppService catUnitAppService) 
        {
            _makeAnAppointmentAppService = makeAnAppointmentAppService;
            _recruitmentAppService = recruitmentAppService;
            _catUnitAppService = catUnitAppService;
        }


        public async Task<IActionResult> Index()
        {
            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
            if (CatUnitRank != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id);
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion
            #region kinh nghiệm
            var CatUnitEX = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Kinh nghiệm làm việc"));
            if (CatUnitEX != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitEX.Id);
                List<SelectListItem> listItemsKinhNghiem = new List<SelectListItem>();
                listItemsKinhNghiem.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKinhNghiem = listItemsKinhNghiem;

            }
            #endregion
            return View();
        }



        public async Task<IActionResult> EditMakeAnAppointment(long Id)
        {
            var makeInFo = await _makeAnAppointmentAppService.GetDetail(Id);
            var model = new MakeAnAppointmentDto();
            model.Id = Id;
            model.InterviewTime = makeInFo.InterviewTime;
            model.Message = makeInFo.Message;
            model.Rank = makeInFo.Rank;
            model.Name = makeInFo.Name;
            model.RecruiterId = makeInFo.RecruiterId;
            model.CandidateId = makeInFo.CandidateId;
            model.JobApplicationId = makeInFo.JobApplicationId;
            model.Address = makeInFo.Address;
            model.ApplicationRequestId = makeInFo.ApplicationRequestId;
            model.TypeInterview = makeInFo.TypeInterview;
            model.InterviewResultLetter = makeInFo.InterviewResultLetter;
            model.StatusOfCandidate = makeInFo.StatusOfCandidate;
            model.ReasonForRefusal = makeInFo.ReasonForRefusal;


            var recruiment = await _recruitmentAppService.GetAllBy();

            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
            if (CatUnitRank != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id);
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion

            List<SelectListItem> listItemsNgheNghiep = new List<SelectListItem>();
            listItemsNgheNghiep.AddRange(recruiment.Items.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString(),
            }));
            ViewBag.listItemsKyThuat = listItemsNgheNghiep;

            return PartialView("EditMakeAnAppointment", model);
        }

        public async Task<IActionResult> MakeAnAppointment(long Id)
        {
            var makeInFo = await _makeAnAppointmentAppService.GetDetail(Id);
            var model = new MakeAnAppointmentDto();
            model.Id = Id;
            model.InterviewTime = makeInFo.InterviewTime;
            model.Message = makeInFo.Message;
            model.Rank = makeInFo.Rank;
            model.Name = makeInFo.Name;
            model.RecruiterId = makeInFo.RecruiterId;
            model.CandidateId = makeInFo.CandidateId;
            model.JobApplicationId = makeInFo.JobApplicationId;
            model.Address = makeInFo.Address;
            model.ApplicationRequestId = makeInFo.ApplicationRequestId;
            model.TypeInterview = makeInFo.TypeInterview;
            model.InterviewResultLetter = makeInFo.InterviewResultLetter;
            model.ReasonForRefusal = makeInFo.ReasonForRefusal;
            var recruiment = await _recruitmentAppService.GetAllBy();

            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
            if (CatUnitRank != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id);
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            #endregion
            List<SelectListItem> listItemsNgheNghiep = new List<SelectListItem>();
            listItemsNgheNghiep.AddRange(recruiment.Items.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString(),
            }));
            ViewBag.listItemsKyThuat = listItemsNgheNghiep;

            return PartialView("ViewMakeAnAppointment", model);
        }

        public async Task<JsonResult> GetAll(MakeAnAppointmentInput input)
        {
            var output = await _makeAnAppointmentAppService.GetAll(input);
            return Json(output);
        }
        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Update(MakeAnAppointmentDto model)
        {
            try
            {
                await _makeAnAppointmentAppService.Update(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        public async Task<JsonResult> Create(MakeAnAppointmentDto model)
        {
             await _makeAnAppointmentAppService.Create(model);
            return Json(model);
        }

    }
}
