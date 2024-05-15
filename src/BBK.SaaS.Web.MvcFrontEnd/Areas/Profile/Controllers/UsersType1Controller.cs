using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
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
        public UsersType1Controller(			
            ILanguageManager languageManager,
            IGeoUnitAppService geoUnitAppService,
            ICatUnitAppService catUnitAppService,
            ICandidateAppService candidateAppService,
			IJobApplicationAppService jobApplicationAppService
			)
		{
			_languageManager = languageManager;
            _geoUnitAppService = geoUnitAppService;
            _catUnitAppService = catUnitAppService;
            _candidateAppService = candidateAppService;
			_jobApplicationAppService = jobApplicationAppService;


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
            var GeoUnit = (await _geoUnitAppService.GetAll()).Where(x => x.ParentId == null);
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
                ViewBag.Occupations = listItemsKyThuat;

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
                ViewBag.Experiences = listItemsKyThuat;

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
                    ViewBag.Literacy = listItems;

                }
            #endregion
            #region Danh mục mức lương
            var CatUnitSalary = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Mức lương")); 
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

        public async Task<IActionResult> CurriculumVitaeJobApplication(long userId)
        {
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
                ViewBag.WorkSite = listItemsKyThuat;

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
                ViewBag.Occupations = listItemsKyThuat;

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
                ViewBag.Experiences = listItemsKyThuat;

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
                ViewBag.Literacy = listItems;

            }
            #endregion
            #region Danh mục mức lương
            var CatUnitSalary = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Mức lương"));
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
            ViewBag.UserId = userId;
            var candidateForEditOutput = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = userId });
            ViewBag.CandidateId = candidateForEditOutput.Candidate.Id ;
            return View("CurriculumVitaeJobApplication" , candidateForEditOutput);
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
			var GeoUnit = (await _geoUnitAppService.GetAll()).Where(x => x.ParentId == null);
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
				usersType1ViewModel.JobApplication.JobAppPartialView.Occupations = listItemsKyThuat;

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
				usersType1ViewModel.JobApplication.JobAppPartialView.Experiences = listItemsKyThuat;

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
					usersType1ViewModel.JobApplication.JobAppPartialView.FormOfWork = listItems;

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
					usersType1ViewModel.JobApplication.JobAppPartialView.Literacy = listItems;

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
    }
}
