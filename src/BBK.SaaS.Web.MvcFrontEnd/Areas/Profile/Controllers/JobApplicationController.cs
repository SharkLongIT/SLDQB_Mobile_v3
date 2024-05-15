using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Auditing;
using Abp.HtmlSanitizer;
using Abp.Web.Models;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Net;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using BBK.SaaS.Web.Controllers;
using BBK.SaaS.Web.Session;
using Ganss.Xss;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Twilio.Rest.Api.V2010.Account;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
    [Area("Profile")]
    [AbpMvcAuthorize]
    public class JobApplicationController : SaaSControllerBase
    {
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly ICandidateAppService _candidateAppService;
        private readonly IPerRequestSessionCache _perRequestSessionCache;
        private readonly IRecruitmentAppService _recruitmentAppService;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly IApplicationRequestAppService _applicationRequestAppService;
        private readonly FileServiceFactory _fileServiceFactory;
        public JobApplicationController(IJobApplicationAppService jobApplicationAppService,
            IPerRequestSessionCache perRequestSessionCache,
            ICandidateAppService candidateAppService,
            IRecruitmentAppService recruitmentAppService,
            ICatUnitAppService catUnitAppService,
            IGeoUnitAppService geoUnitAppService,
            IApplicationRequestAppService applicationRequestAppService,
            FileServiceFactory fileServiceFactory)
        {
            _jobApplicationAppService = jobApplicationAppService;
            _candidateAppService = candidateAppService;
            _perRequestSessionCache = perRequestSessionCache;
            _recruitmentAppService = recruitmentAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
            _applicationRequestAppService = applicationRequestAppService;
            _fileServiceFactory = fileServiceFactory;
        }
        public async Task<IActionResult> UserJob()
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
            var CatUnitFormOfWork = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Hình thức làm việc")); 
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
            return View();
        }

        public async Task<IActionResult> CreateJobOfCandidate(long? jobAppId)
        {
            var candidate = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = AbpSession.UserId });

            if(jobAppId != null)
            {
                var Job = await _jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = jobAppId.Value });
                if(Job != null)
                {
					if(Job.JobApplication.CandidateId != candidate.Candidate.Id)
                    {
						return RedirectToAction("Index", "Home", new { area = "" });
					}
				}
            }
            JobApplicationModel JobApplicationModel = new JobApplicationModel();
            JobApplicationModel.JobAppPartialView = new JobAppPartialView();
            if (candidate != null)
            {
                if (candidate.User.Id == AbpSession.UserId)
                {
                    JobApplicationModel.JobAppPartialView.CanUpdate = true;
                }
                JobApplicationModel.JobAppPartialView.CandidateId = candidate.Candidate.Id;
            }
            JobApplicationModel.JobAppPartialView.Id = jobAppId;



            #region Địa điểm làm việc 
            var GeoUnit = (await _geoUnitAppService.GetAll()).Where(x => x.ParentId == null);
            if (GeoUnit != null)
            {

                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(GeoUnit.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                JobApplicationModel.JobAppPartialView.WorkSite = listItemsKyThuat;

            }
            #endregion
            #region Danh mục nghề nghiệp
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
                JobApplicationModel.JobAppPartialView.Occupations = listItemsKyThuat;

            }
            #endregion
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
                JobApplicationModel.JobAppPartialView.Experiences = listItemsKyThuat;

            }
            #endregion
            #region Danh mục hình thức làm việc 
            var CatUnitFormOfWork = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Hình thức làm việc")); 
                if (CatUnitFormOfWork != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitFormOfWork.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    JobApplicationModel.JobAppPartialView.FormOfWork = listItems;

                }
            #endregion
            #region Danh mục bằng cấp
            var CatUnitLiteracy = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Bằng cấp"));
                if (CatUnitLiteracy != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    JobApplicationModel.JobAppPartialView.Literacy = listItems;

                }
            #endregion
            #region Danh mục Cấp bậc mong muốn
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
                    JobApplicationModel.JobAppPartialView.Positions = listItems;

                }
            #endregion

            #region Mẫu Pdf 
            List<Templatepdf> widgettemps = null;
            using (var fileService = _fileServiceFactory.Get())
            {


                //var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
                var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CVTemplates\\TemplateCV.json" });
                widgettemps = JsonConvert.DeserializeObject<List<Templatepdf>>(Encoding.UTF8.GetString(fileMgr.Content));
            }

            ViewBag.Template = widgettemps;
            #endregion


            return View(JobApplicationModel);
        }
        public async Task<IActionResult> PreViewJobOfCandidate(long? jobAppId)
        {
            var cadidate = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = AbpSession.UserId });
            ViewBag.candidateId = cadidate.Candidate.Id;
            ViewBag.jobAppId = jobAppId;
            return View();
        }
        public async Task<PartialViewResult> PartialViewWorkExp()
        {
            return PartialView("PartialViewWorkExp");
        }
        public async Task<PartialViewResult> PartialViewLearnProcess()
        {


            return PartialView("PartialViewLearnProcess");
        }
        public async Task<PartialViewResult> ViewJobApp()
        {

            return PartialView("ViewJobApp");
        }
        public async Task<IActionResult> PartialViewCommonJobApp(long? id)
        {
            JobAppPartialView JobAppPartialView = new JobAppPartialView();

            JobAppPartialView.Id = id;
            #region Địa điểm làm việc 
            var GeoUnit = _geoUnitAppService.GetAll().Result.Where(x => x.ParentId == null);
            if (GeoUnit != null)
            {

                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(GeoUnit.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                JobAppPartialView.WorkSite = listItemsKyThuat;

            }
            #endregion
            #region Danh mục nghề nghiệp
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
                JobAppPartialView.Occupations = listItemsKyThuat;

            }
            #endregion
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
                JobAppPartialView.Experiences = listItemsKyThuat;

            }
            #endregion
            #region Danh mục hình thức làm việc 
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
                    JobAppPartialView.FormOfWork = listItems;

                }
            #endregion
            #region Danh mục bằng cấp
            var CatUnitLiteracy = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault(); if (CatUnit != null)
                if (CatUnitLiteracy != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    JobAppPartialView.Literacy = listItems;

                }
            #endregion
            #region Danh mục Cấp bậc mong muốn
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
                    JobAppPartialView.Positions = listItems;
                }
            #endregion
            return PartialView("PartialViewCommonJobApp", JobAppPartialView);
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
            }
            GetAllJobOfCandidate viewModel = new GetAllJobOfCandidate();
            viewModel.Candidate = viewModels;
            viewModel.Count = candidateDto.TotalCount;
            var usercurrent = await _perRequestSessionCache.GetCurrentLoginInformationsAsync();
            if (usercurrent != null)
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
            return Json(viewModel);
        }

        public async Task<IActionResult> Detail(long Id)
        {
            JobApplicationModel jobApplicationModel = new JobApplicationModel();
            var jobApp = await _jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = Id });

            if(AbpSession.UserId != jobApp.Candidate.UserId)
            {
                if(jobApp.JobApplication.IsPublished == true)
                {
					return RedirectToAction("Detail", "UserJob", new {  Id = jobApp.JobApplication.Id , Area = "" });
                }
                else
                {
					return RedirectToAction("Index", "Home", new { area = "" });
				}
			}
            jobApplicationModel = ObjectMapper.Map<JobApplicationModel>(jobApp);
			if (jobApplicationModel.JobApplication.FileMgr != null)
			{
				if (jobApplicationModel.JobApplication.FileMgr.FilePath != null)
				{
					jobApplicationModel.JobApplication.FileMgr.FileUrl = $"/file/get?c=" + jobApplicationModel.JobApplication.FileMgr.FilePath;
                }
			
			}
			return View(jobApplicationModel);
        }
       

        public async Task<JsonResult> GetJobApp(long id)
        {
            try
            {
                var JobApplication = await _jobApplicationAppService.GetJobApplication(new NullableIdDto<long> { Id = id });

                if(JobApplication.FileMgr != null)
                {
                    if(JobApplication.FileMgr.FilePath != null)
                   {
                        //JobApplication.FileMgr.FileUrl = $"/file/get?c=" + {StringCipher.Instance.Decrypt(JobApplication.FileMgr.FilePath)};
                        JobApplication.FileMgr.FileUrl =  $"/file/get?c=" + StringCipher.Instance.Encrypt(JobApplication.FileMgr.FilePath);
                        JobApplication.FileMgr.FileName =  JobApplication.FileMgr.FileName;
                        JobApplication.FileMgr.FilePath = StringCipher.Instance.Encrypt(JobApplication.FileMgr.FilePath);
                    }
                    int tenantId = AbpSession.TenantId ?? 1;
                }
                JobApplication.FileCVUrl = StringCipher.Instance.Encrypt(JobApplication.FileCVUrl);

                return Json(JobApplication);
            }
            catch (Exception ex)
            {

                return Json(new AjaxResponse(new ErrorInfo(ex.Message))); ;
            }

        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> CreateJobApp(JobApplicationCreate JobApplicationCreate)
        {
            try
            {
                var JobApplication = await _jobApplicationAppService.CreateJobApplicationForWeb(JobApplicationCreate);
                return Json(JobApplication);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> CreateWorkExp(WorkExperienceEditDto WorkExperienceEditDto)
        {
            try
            {
                var WorkExperienc = await _jobApplicationAppService.CreateWorkExperience(WorkExperienceEditDto);
                return Json(WorkExperienc);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> CreateLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.CreateLearningProcess(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> UpdateJobApplication(JobApplicationCreate input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.UpdateJobApplicationForWeb(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> UpdateLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.UpdateLearningProcess(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }


        public async Task<JsonResult> GetListWorkExp(long Id)
        {
            try
            {
                var ListWorkExp = await _jobApplicationAppService.GetWorkExperiencesForList(new NullableIdDto<long> { Id = Id });
                return Json(ListWorkExp);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        
        public async Task<JsonResult> GetListLearningProcess(long Id)
        {
            try
            {
                var ListWorkExp = await _jobApplicationAppService.GetLearningProcessForList(new NullableIdDto<long> { Id = Id });
                return Json(ListWorkExp);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
        
        public async Task<JsonResult> GetWorkExp(long Id)
        {
            try
            {
                var ListWorkExp = await _jobApplicationAppService.GetWorkExperience(new NullableIdDto<long> { Id = Id });
                return Json(ListWorkExp);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }
       
        public async Task<JsonResult> GetLearnProcess(long Id)
        {
            try
            {
                var LearnProcess = await _jobApplicationAppService.GetLearningProcess(new NullableIdDto<long> { Id = Id });
                return Json(LearnProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> UpdateWorkExp(WorkExperienceEditDto input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.UpdateWorkExperience(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> DeleteWorkExp(WorkExperienceEditDto input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.DeleteWorkExperience(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> DeleteJobApplication(NullableIdDto<long> input)
        {
            try
            {
                var JobApplication = await _jobApplicationAppService.DeleteJobApplication(input);
                return Json(JobApplication);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> DeleteLearningProcess(LearningProcessEditDto input)
        {
            try
            {
                var LearningProcess = await _jobApplicationAppService.DeleteLearningProcess(input);
                return Json(LearningProcess);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        public async Task<JsonResult> GetListJobAppOfCandidate(JobAppSearch input)
        {
            try
            {
                var JobAppOfCandidate = await _jobApplicationAppService.GetListJobAppOfCandidate(input);
                return Json(JobAppOfCandidate);
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

        public async Task<JsonResult> updatePushlishById(long id)
        {
            try
            {
                await _jobApplicationAppService.UpdatePushlishById(new NullableIdDto<long> { Id = id});
                return Json(Ok());
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }



        [DisableRequestSizeLimit]
        public async Task<JsonResult> UploadFile(IFormFile file, long recruiterId)
        {
            //if (!ModelState.IsValid)
            //	throw new UserFriendlyException("ModelState Invalid");

            var results = new FileUploadSummary();

            if (file != null && file.Length > 0)
            {
                using (var streamContent = file.OpenReadStream())
                {
                    FileMgr fileInput = new FileMgr();
                    fileInput.FileName = file.FileName;
                    fileInput.TenantId = AbpSession.TenantId.Value;
                    fileInput.CreatedAt = DateTime.Now;
                    fileInput.FileCategory = "JobApp";

                    using (var fileService = _fileServiceFactory.Get())
                    {
                        fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);

                        fileInput.FileUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath));
                        fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);
                    }

                    results.Files.Add(fileInput);
                }
            }

            return Json(results);
        }

        //Model Dat Lich
        public async Task<IActionResult> MakeAnAppointment(long JobId , long? RecruimentId)
        {
            var JobInfo = await _jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = JobId });
            var recruiter = await _perRequestSessionCache.GetRecruiter();
            var recruiment = await _recruitmentAppService.GetAllBy();
            var model = new MakeAnAppointmentDto();
            model.JobApplicationId = JobId;
            if (RecruimentId.HasValue)
            {
                model.ApplicationRequestId = RecruimentId.Value;
            }

            if (JobInfo != null)
            {
                model.CandidateId = JobInfo.Candidate.Id.Value;
            }
            if (recruiter != null)
            {
                model.RecruiterId = recruiter.Id;
            }

            #region cấp bậc
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));
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

            List<SelectListItem> listItemsNgheNghiep = new List<SelectListItem>();
            listItemsNgheNghiep.AddRange(recruiment.Items.Select(x => new SelectListItem
            {
                Text = x.Title,
                Value = x.Id.ToString(),
            }));
            ViewBag.listItemsKyThuat = listItemsNgheNghiep;
            return PartialView("MakeAnAppointment", model);
        }

    }
}
