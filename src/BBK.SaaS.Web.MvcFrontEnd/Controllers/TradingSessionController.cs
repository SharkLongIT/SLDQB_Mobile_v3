using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.HtmlSanitizer;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Web.Models.TradingSession;
using BBK.SaaS.Web.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
    public class TradingSessionController : SaaSControllerBase
    {
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly ITradingSessionAccountAppService _tradingSessionAccountAppService;
        private readonly ITradingSessionAppService _tradingSessionAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IRecruitmentAppService _recruitmentAppService;
        private readonly IJobApplicationAppService _IJobApplicationAppService;


        public TradingSessionController(IGeoUnitAppService geoUnitAppService, ITradingSessionAccountAppService tradingSessionAccountAppService,
            IPerRequestSessionCache perRequestSessionCache, ITradingSessionAppService tradingSessionAppService, IRecruitmentAppService recruitmentAppService, IJobApplicationAppService IJobApplicationAppService)
        {
            _geoUnitAppService = geoUnitAppService;
            _tradingSessionAccountAppService = tradingSessionAccountAppService;
            _perRequestSessionCache = perRequestSessionCache;
            _tradingSessionAppService = tradingSessionAppService;
            _recruitmentAppService = recruitmentAppService;
            _IJobApplicationAppService = IJobApplicationAppService;
        }
        [AbpAuthorize]
        public async Task<IActionResult> Index(TradingSessionSearch input, int? page = 0)
        {
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

            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            tradingSessionSearch.Search = input.Search;
            tradingSessionSearch.WorkSite = input.WorkSite;
            tradingSessionSearch.FromDate = input.FromDate;
            tradingSessionSearch.ToDate = input.ToDate;
            tradingSessionSearch.Status = input.Status;
            var TradingList = (await _tradingSessionAppService.GetAll(tradingSessionSearch)).Items.
                Where(x => x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now || x.StartTime > DateTime.Now).ToList();
            var viewModels = ObjectMapper.Map<List<TradingViewModel>>(TradingList);

            #region phan trang
            int limit = 8;
            int start;
            if (page > 0)
            {
                page = page;
            }
            else
            {
                page = 1;
            }
            start = (int)(page - 1) * limit;

            ViewBag.pageCurrent = page;

            int totalProduct = viewModels.Count();

            ViewBag.totalProduct = totalProduct;

            ViewBag.numberPage = (int)Math.Ceiling((float)totalProduct / limit);
            #endregion

            GetAllTrading viewModel = new GetAllTrading();
            viewModel.Trading = viewModels.Skip(start).Take(limit).ToList();


            SelectListItem selListItem = new SelectListItem() { Value = "1", Text = "Đang diễn ra" };
            SelectListItem selListItem2 = new SelectListItem() { Value = "2", Text = "Sắp diễn ra" };

            List<SelectListItem> newList = new List<SelectListItem>();
            newList.Add(selListItem);
            newList.Add(selListItem2);

            ViewBag.newList = newList;

            return View(viewModel);
        }

        [AbpAuthorize]
        public async Task<IActionResult> ViewDetailTradingSessionAccount(long? Id)
        {
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            TradingViewModel viewModel = new TradingViewModel();
            var TradingDetail = (await _tradingSessionAppService.GetAll(tradingSessionSearch)).Items.FirstOrDefault(x => x.Id == Id);
            if (TradingDetail != null)
            {
                viewModel.Id = Id;
                viewModel.NameTrading = TradingDetail.NameTrading;
                viewModel.Province = TradingDetail.Province;
                viewModel.StartTime = TradingDetail.StartTime;
                viewModel.EndTime = TradingDetail.EndTime;
                viewModel.Describe = TradingDetail.Describe;
                viewModel.ImgUrl = TradingDetail.ImgUrl;
                viewModel.Description = TradingDetail.Description;
                viewModel.CountCandidateMax = TradingDetail.CountCandidateMax;
                viewModel.CountRecruiterMax = TradingDetail.CountRecruiterMax;
            }


            // danh sach nha tuyen dung
            TradingSessionAccountSeach tradingSessionAccountSeach = new TradingSessionAccountSeach();
            tradingSessionAccountSeach.Id = Id;
            var tradingAccountRecruiter = (await _tradingSessionAccountAppService.GetAllRecuiter(tradingSessionAccountSeach)).Items.Where(x => x.TradingSessionId == Id);
            ViewBag.Recruiter = tradingAccountRecruiter;
            ViewBag.RecruiterCount = tradingAccountRecruiter.Count();


            // danh sach nguoi lao donh
            var tradingAccountCandidate = (await _tradingSessionAccountAppService.GetAllCandidate(tradingSessionAccountSeach)).Items;
            ViewBag.Candidate = tradingAccountCandidate;
            ViewBag.CandidateCount = tradingAccountCandidate.Count();

            // danh sach phien giao dich khac
            var TradingList = (await _tradingSessionAppService.GetAll(tradingSessionSearch)).Items.Where(x => x.Id != Id).
                Where(x => x.StartTime <= DateTime.Now && x.EndTime >= DateTime.Now || x.StartTime > DateTime.Now).ToList();
            var viewModels = ObjectMapper.Map<List<TradingViewModel>>(TradingList);
            ViewBag.TradingList = viewModels.Take(4).ToList();


            // danh sach tin tuyen dung cua nha tuyen dung
            RecruimentInput recruiterSearch = new RecruimentInput();
            var rewcruimentDto = (await _recruitmentAppService.GetAllByAllUser(recruiterSearch)).Items.Where(x=> tradingAccountRecruiter.Select(x=>x.RecruiterId).Contains(x.RecruiterId));
            ViewBag.Recruiment = rewcruimentDto;
            ViewBag.RecruimentCount = rewcruimentDto.Count();


           var userCurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (userCurrent.User != null)
            {
                if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                {
                    if(TradingDetail.CountRecruiterMax == tradingAccountRecruiter.Count())
                    {
                        viewModel.IsCheckMax = true;
                    }
                }
                if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                {
                    if (TradingDetail.CountCandidateMax == tradingAccountCandidate.Count())
                    {
                        viewModel.IsCheckMax = true;
                    }
                }
            }

            return View(viewModel);
        }

        // Create TradingSessionAcount
        [AbpMvcAuthorize]
        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Create(TradingSessionAccountEditDto model)
        {
            try
            {
                var Recruiter = await _perRequestSessionCache.GetRecruiter();
                var Candidate = await _perRequestSessionCache.GetCandidate();
                JobAppSearch jobAppSearch = new JobAppSearch();
                var userCurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
                if (userCurrent.User != null)
                {
                    if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type1)
                    {
                        model.UsertId = userCurrent.User.Id;
                        model.RecruiterId = Recruiter.Id;
                    }
                    if (userCurrent.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                    {
                        var job = (await _IJobApplicationAppService.GetAllJobApps(jobAppSearch)).Items.FirstOrDefault(x=>x.Candidate.Id == Candidate.Id);

                        model.UsertId = userCurrent.User.Id;
                        model.CandidateId = Candidate.Id;

                        if(job != null && job.JobApplication.Id.HasValue)
                        {
                            model.JobApplicationId = job.JobApplication.Id.Value;
                        }

                    }
                }

                var tradingaccount = await _tradingSessionAccountAppService.Create(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        //update
        [AbpMvcAuthorize]
        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> UpdateStatus(TradingSessionAccountEditDto model)
        {
            try
            {
                var tradingaccount = _tradingSessionAccountAppService.UpdateStatusByTradingId(model);
                return Json(model);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }


        public async Task<JsonResult> GetTrading()
        {
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            var trading = _tradingSessionAccountAppService.GetAll(tradingSessionSearch).Result.Items;
            return Json(trading);
        }


        public int Check(long? TradingSessionId)
        {
            var result = _tradingSessionAccountAppService.CheckAccount(TradingSessionId);
            return result;
        }


        public async Task<JsonResult> GetByChart(long Id)
        {
            var trading = await _tradingSessionAccountAppService.GetByChart(Id);
            return Json(trading);
        }

    }
}
