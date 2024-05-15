using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using BBK.SaaS.Web.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
    public class UserJobController : SaaSControllerBase
    {
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly ICandidateAppService _candidateAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IRecruitmentAppService _recruitmentAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IGeoUnitAppService _geoUnitAppService;

        public UserJobController(IJobApplicationAppService jobApplicationAppService, ICandidateAppService candidateAppService, IPerRequestSessionCache perRequestSessionCache, IRecruitmentAppService recruitmentAppService, ICatUnitAppService catUnitAppService, IGeoUnitAppService geoUnitAppService)
        {
            _jobApplicationAppService = jobApplicationAppService;
            _candidateAppService = candidateAppService;
            _perRequestSessionCache = perRequestSessionCache;
            _recruitmentAppService = recruitmentAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
        }


        #region Người tìm việc
        public async Task<IActionResult> Index()
        {
            var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
            if (CatUnit != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnit.Id).Result;
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #region Danh mục kinh nghiệm làm việc 
            var CatUnitWorkExp = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Kinh nghiệm làm việc")).FirstOrDefault();
            if (CatUnitWorkExp != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitWorkExp.Id).Result;
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.Experiences = listItemsKyThuat;

            }
            #endregion
            var CatUnitFormOfWork = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Hình thức làm việc")).FirstOrDefault(); if (CatUnit != null)
                if (CatUnitFormOfWork != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemFormOfWork = listItems;

                }
            var CatUnitLiteracy = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnit != null)
                if (CatUnitLiteracy != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemsCatUnitLiteracy = listItems;

                }
            var CatUnitPositionsId = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault(); if (CatUnit != null)
                if (CatUnitPositionsId != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitPositionsId.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.listItemPositions = listItems;

                }
            return View("UserJob");
        }
        public async Task<JsonResult> GetUserJob(CandidateInput input)
        {


            JobAppSearch candidateSearch = new JobAppSearch();

            if (input != null)
            {
                candidateSearch.Take = input.PageSize;
                candidateSearch.Paging = input.Page;
                candidateSearch.Search = input.Search;
                candidateSearch.WorkSite = input.WorkSite;
                if (input.OccupationId != 0)
                {
                    candidateSearch.OccupationId = input.OccupationId;
                }
                if (input.LiteracyId != 0)
                {
                    candidateSearch.LiteracyId = input.LiteracyId;
                }
                if (input.Gender.HasValue) { candidateSearch.Gender = input.Gender; }
                if (input.ExperiencesId != 0) candidateSearch.ExperiencesId = input.ExperiencesId;



                if (input.Page > 1)
                {
                    candidateSearch.SkipCount = input.PageSize;

                }
            }
            int limit = 0;

            if (input.PageSize <= 0)
            {
                limit = input.Page;
            }
            int start;
            if (input.Page > 0)
            {
                input.Page = input.Page;
            }
            else
            {
                input.Page = 1;
            }
            start = (int)(input.Page - 1) * limit;
            ViewBag.pageCurrent = input.Page;

            var candidateDto = await _jobApplicationAppService.GetAllJobApps(candidateSearch);



            var viewModels = ObjectMapper.Map<List<JobApplicationModel>>(candidateDto.Items);
            foreach (var item in viewModels)
            {
                item.CandidateAge = item.Candidate.DateOfBirth.HasValue ? (int)(DateTime.Now - item.Candidate.DateOfBirth.Value).TotalDays / 365 : -1;
                item.ProfilePictureId = item.ProfilePictureId;
            }
            GetAllJobOfCandidate viewModel = new GetAllJobOfCandidate();
            viewModel.Candidate = viewModels;
            viewModel.Count = candidateDto.TotalCount;
            var usercurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (usercurrent != null)
            {
                if (usercurrent.User != null)
                {
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                    {
                        viewModel.IsRecruiters = true;
                    }
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                    {
                        viewModel.IsCandidate = true;
                    }
                }

            }
            return Json(viewModel);
        }
        public async Task<IActionResult> Detail(long Id)
        {
            JobApplicationModel jobApplicationModel = new JobApplicationModel();
            var jobApp = await _jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = Id });
            jobApplicationModel = ObjectMapper.Map<JobApplicationModel>(jobApp);
            return View("JobApplicationDetail", jobApplicationModel);
        }
        #endregion
    }
}
