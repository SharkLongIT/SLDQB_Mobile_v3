using BBK.SaaS.Identity;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
    public class HomeController : SaaSControllerBase
    {
        private readonly SignInManager _signInManager;
        private readonly IRecruitmentAppService _recruitmentAppSerVice;
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly ITradingSessionAppService _tradingSessionAppService;
        private readonly IArticlesAppService _articlesAppService;

        public HomeController(SignInManager signInManager, IRecruitmentAppService recruitmentAppSerVice, 
            IJobApplicationAppService jobApplicationAppService, 
            ICatUnitAppService catUnitAppService, 
            IGeoUnitAppService geoUnitAppService,
			 ITradingSessionAppService tradingSessionAppService, IArticlesAppService articlesAppService)
        {
            _signInManager = signInManager;
            _recruitmentAppSerVice = recruitmentAppSerVice;
            _jobApplicationAppService = jobApplicationAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
            _tradingSessionAppService = tradingSessionAppService;
            _articlesAppService = articlesAppService;
        }

        public async Task<IActionResult> Index(string redirect = "", bool forceNewRegistration = false)
        {
            //if (forceNewRegistration)
            //{
            //    await _signInManager.SignOutAsync();
            //}

            //if (redirect == "TenantRegistration")
            //{
            //    return RedirectToAction("SelectEdition", "TenantRegistration");
            //}

            //return AbpSession.UserId.HasValue ?
            //    RedirectToAction("Index", "Home", new { area = "AppAreaName" }) :
            //    RedirectToAction("Login", "Account");
            #region chuyên môn kỹ thuật
            var CatUnit = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
            if (CatUnit != null)
            {
                var CatParent = await _catUnitAppService.GetChildrenCatUnit(CatUnit.Id);
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsKyThuat = listItemsKyThuat;

            }
            #endregion

            #region tinh thanh
            var List = (await _geoUnitAppService.GetAll()).Where(x => x.ParentId == null);
            if (List != null)
            {

                List<SelectListItem> listItemsTinhThanh = new List<SelectListItem>();
                listItemsTinhThanh.AddRange(List.Select(x => new SelectListItem
                {
                    Text = x.DisplayName,
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsTinhThanh = listItemsTinhThanh;

            }
            #endregion

            #region tin tuyen dung
            RecruimentInput recruimentInput = new RecruimentInput();
            var rewcruimentDto = await _recruitmentAppSerVice.GetAllByAllUser(recruimentInput);
            var viewModels = ObjectMapper.Map<List<JobUserModel>>(rewcruimentDto.Items.Take(6));
            #endregion

            #region ho so tuyen dung
            JobAppSearch jobAppSearch = new JobAppSearch();
            var candidateDto = await _jobApplicationAppService.GetAllJobApps(jobAppSearch);
            var candidate = ObjectMapper.Map<List<JobApplicationModel>>(candidateDto.Items.Take(6));
            foreach (var item in candidate)
            {
                TimeSpan time = DateTime.Now - item.JobApplication.CreationTime;

                item.Age = GetTimeSince(item.JobApplication.CreationTime);
            }
            #endregion

            #region Phien giao dich
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            var tradingList = (await _tradingSessionAppService.GetAll(tradingSessionSearch)).Items
                .Where(x => x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now || x.StartTime > DateTime.Now).Take(4).ToList();
            #endregion

            #region Tin tuc
            SearchArticlesInput searchArticlesInput = new SearchArticlesInput();
            var ListArticle = (await _articlesAppService.GetFEArticles(searchArticlesInput)).Items.OrderBy(x=>x.Id).Take(2).ToList();
            #endregion

            GetAllJobOfUser viewModel = new GetAllJobOfUser();
            viewModel.Recruiment = viewModels;
            viewModel.Candidate = candidate;
            viewModel.TradingSession = tradingList;
            viewModel.Article = ListArticle;
            return View(viewModel);
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
                    String.Format("{0} tháng trước",Math.Round(Math.Round(timeSpan.TotalDays) / 30), MidpointRounding.AwayFromZero) :
                    "1 tháng trước";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} năm trước", Math.Round(Math.Round(timeSpan.TotalDays) / 365), MidpointRounding.AwayFromZero) :
                    "1 năm trước";
            }

            return result;
        }
    }
}