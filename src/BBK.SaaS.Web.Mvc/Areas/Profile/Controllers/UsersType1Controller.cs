using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType0;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType1;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using Twilio.Rest.Api.V2010.Account;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Profile")]
    [AbpMvcAuthorize]
    public class UsersType1Controller : SaaSControllerBase
    {
		private readonly ILanguageManager _languageManager;
        private readonly IGeoUnitAppService _geoUnitAppService; 
        private readonly ICatUnitAppService _catUnitAppService; 
        private readonly ICandidateAppService _candidateAppService; 
		private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly INVNVCandidateAppService _NVNVCandidateAppService;
        public UsersType1Controller(			
            ILanguageManager languageManager,
            IGeoUnitAppService geoUnitAppService,
            ICatUnitAppService catUnitAppService,
            ICandidateAppService candidateAppService,
			IJobApplicationAppService jobApplicationAppService,
            INVNVCandidateAppService NVNVCandidateAppService
            )
		{
			_languageManager = languageManager;
            _geoUnitAppService = geoUnitAppService;
            _catUnitAppService = catUnitAppService;
            _candidateAppService = candidateAppService;
			_jobApplicationAppService = jobApplicationAppService;
            _NVNVCandidateAppService = NVNVCandidateAppService;


        }
		public IActionResult Index()
        {
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}
			var viewModel = new UsersType1ViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
        }

		public async Task<IActionResult> Detail(long id)
		{
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}
			var viewModel = new RecruiterDetailViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
		}

		public async Task<IActionResult> CurriculumVitae()
		{
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
                ViewBag.WorkSite = listItemsKyThuat;

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
                ViewBag.Occupations = listItemsKyThuat;

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
                ViewBag.Experiences = listItemsKyThuat;

            }
            #endregion
            #region Danh mục bằng cấp
            var CatUnitLiteracy = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault(); 
                if (CatUnitLiteracy != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.Literacy = listItems;

                }
            #endregion
            #region Danh mục mức lương
            var CatUnitSalary = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Mức lương")).FirstOrDefault(); 
                if (CatUnitSalary != null)
                {
                    var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitSalary.Id).Result;
                    List<SelectListItem> listItems = new List<SelectListItem>();
                    listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                    {
                        Text = L(x.DisplayName),
                        Value = x.Id.ToString(),
                    }));
                    ViewBag.Salary = listItems;

                }
            #endregion
            return View("CurriculumVitae");
		}

        public async Task<IActionResult> CurriculumVitaeJobApplication()
        {
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
                ViewBag.WorkSite = listItemsKyThuat;

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
                ViewBag.Occupations = listItemsKyThuat;

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
                ViewBag.Experiences = listItemsKyThuat;

            }
            #endregion
            #region Danh mục bằng cấp
            var CatUnitLiteracy = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Bằng cấp")).FirstOrDefault();
            if (CatUnitLiteracy != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitLiteracy.Id).Result;
                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.Literacy = listItems;

            }
            #endregion
            #region Danh mục mức lương
            var CatUnitSalary = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Mức lương")).FirstOrDefault();
            if (CatUnitSalary != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitSalary.Id).Result;
                List<SelectListItem> listItems = new List<SelectListItem>();
                listItems.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.Salary = listItems;

            }
            #endregion
           
            return View("CurriculumVitaeJobApplication");
        }
        [AbpMvcAuthorize]

        public async Task<IActionResult> CurriculumVitaeDetail(long JobAppId)
        {
			UsersType1ViewModel usersType1ViewModel = new UsersType1ViewModel();

			//var candidate = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = AbpSession.UserId });



			usersType1ViewModel.JobApplication = new JoApplicationOfUsersType1ViewModel();


			usersType1ViewModel.JobApplication.JobAppPartialView = new Profile.Models.UsersType1.JobAppPartialView();
			
			usersType1ViewModel.JobApplication.JobAppPartialView.Id = JobAppId;

			var JobApp = await _jobApplicationAppService.GetJobApplicationForEdit(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = JobAppId});


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
				usersType1ViewModel.JobApplication.JobAppPartialView.WorkSite = listItemsKyThuat;

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
				usersType1ViewModel.JobApplication.JobAppPartialView.Occupations = listItemsKyThuat;

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
				usersType1ViewModel.JobApplication.JobAppPartialView.Experiences = listItemsKyThuat;

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
					usersType1ViewModel.JobApplication.JobAppPartialView.FormOfWork = listItems;

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
					usersType1ViewModel.JobApplication.JobAppPartialView.Literacy = listItems;

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
					usersType1ViewModel.JobApplication.JobAppPartialView.Positions = listItems;

				}
			#endregion

			
            var candidateDto = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(JobApp.Candidate.UserId));

            ViewBag.Province = candidateDto.Candidate.ProvinceId;
            ViewBag.District = candidateDto.Candidate.DistrictId;

             usersType1ViewModel = ObjectMapper.Map<UsersType1ViewModel>(candidateDto);

            if (usersType1ViewModel.Candidate.UserId == AbpSession.UserId)
            {
				usersType1ViewModel.CanUpdateAvatar = true;
            }

			usersType1ViewModel.JobApplication = ObjectMapper.Map<JoApplicationOfUsersType1ViewModel>(JobApp); ;


			return View(usersType1ViewModel);
        }

        public async Task<JsonResult> GetCandidate(string Search)
        {
            var candidate = _NVNVCandidateAppService.GetAllCandidateOfProfessionalStaff(new JobAppSearchOfProfessionalStaff() { Search = Search });
           return Json(candidate);
        }
    }
}
