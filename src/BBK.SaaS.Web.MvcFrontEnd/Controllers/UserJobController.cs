using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Security;
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
            var CatUnit = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
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
            var CatUnitWorkExp = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Kinh nghiệm làm việc"));
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
            var CatUnitFormOfWork = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Hình thức làm việc")); if (CatUnit != null)
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
            var CatUnitLiteracy = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Bằng cấp"));
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
            var CatUnitPositionsId = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
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
                candidateSearch.SalaryMax = input.SalaryMax;
                candidateSearch.SalaryMin = input.SalaryMin;
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


            foreach (var item in viewModel.Candidate)
            {
                item.LastModificationTimeString = GetTimeSince(item.JobApplication.CreationTime);
            }
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

            if (jobApplicationModel.JobApplication.FileMgr != null)
            {
                if (jobApplicationModel.JobApplication.FileMgr.FilePath != null)
                {
                    jobApplicationModel.JobApplication.FileMgr.FileUrl = $"/file/get?c=" + StringCipher.Instance.Encrypt(jobApplicationModel.JobApplication.FileMgr.FilePath);
                }

            }
            var usercurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (usercurrent != null)
            {
                if (usercurrent.User != null)
                {
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                    {
                        ViewBag.IsRecruiters = true;
                    }
                    if (usercurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                    {
                        ViewBag.IsCandidate = true;
                    }
                }

            }
            return View("JobApplicationDetail", jobApplicationModel);
        }

        public async Task<JsonResult> GetGeoUnit()
        {
            var geoUnit = (await _geoUnitAppService.GetGeoUnits()).Items.Where(x => x.ParentId == null);
            return Json(geoUnit);
        }
        public async Task<JsonResult> GetGeoUnitChildren(long id)
        {
            var geoUnit = (await _geoUnitAppService.GetGeoUnits()).Items.Where(x => x.ParentId == id);
            return Json(geoUnit);
        }

        public static string GetTimeSince(DateTime objDateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(objDateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} giây trước", Math.Round(timeSpan.TotalSeconds));
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("{0} phút trước", Math.Round(timeSpan.TotalMinutes)) :
                    "1 phút trước";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("{0} giờ trước", Math.Round(timeSpan.TotalHours)) :
                    "1 giờ trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("{0} ngày trước", Math.Round(timeSpan.TotalDays)) :
                    "1 ngày trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} tháng trước", Math.Round(Math.Round(timeSpan.TotalDays) / 30), MidpointRounding.AwayFromZero) :
                    "1 tháng trước";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} năm trước", Math.Round(Math.Round(timeSpan.TotalDays) / 365), MidpointRounding.AwayFromZero) :
                    "1 năm trước";
            }

            return result;
            #endregion
        }
    }
}
