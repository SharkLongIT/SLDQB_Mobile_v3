using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Localization;
using Abp.Runtime.Security;
using Abp.Web.Models;
using BBK.SaaS.Authorization;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType0;
using BBK.SaaS.Web.Controllers;
using GraphQL;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Web;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
    [Area("Profile")]

    public class CandidateController : SaaSControllerBase
    {
        private readonly ICandidateAppService _candidateAppService;
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly ILanguageManager _languageManager;
        private readonly ICatUnitAppService _catUnitAppService;
        private readonly IGeoUnitAppService _geoUnitAppService;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IMakeAnAppointmentAppService _makeAnAppointmentAppService;

        public CandidateController(
            ICandidateAppService candidateAppService,
            ILanguageManager languageManager,
            FileServiceFactory fileServiceFactory,
            IJobApplicationAppService jobApplicationAppService,
            ICatUnitAppService catUnitAppService,
            IGeoUnitAppService geoUnitAppService,
            IMakeAnAppointmentAppService makeAnAppointmentAppService
            )
        {
            _candidateAppService = candidateAppService;
            _languageManager = languageManager;
            _fileServiceFactory = fileServiceFactory;
            _jobApplicationAppService = jobApplicationAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
            _makeAnAppointmentAppService = makeAnAppointmentAppService;
        }

        [AbpMvcAuthorize]
        public async Task<IActionResult> Detail(long id)
        {
			if (!PermissionChecker.IsGranted(AppPermissions.Pages_Administration_Users_Edit))
			{
				return RedirectToAction("Index", "Home", new { area = "" });
			}
            var candidateDto = await _candidateAppService.GetCandidateForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));

            ViewBag.Province = candidateDto.Candidate.ProvinceId;
            ViewBag.District = candidateDto.Candidate.DistrictId;



            #region Encryption

            #endregion
            var viewModel = ObjectMapper.Map<CandidateDetailViewModel>(candidateDto);

            if (candidateDto.Candidate.UserId == AbpSession.UserId)
            {
                viewModel.CanUpdateAvatar = true;
            }
            return View(viewModel);
        }
        [AbpMvcAuthorize]
        public async Task<IActionResult> JobAppOfCandidate()
        {


            var GeoUnit = _geoUnitAppService.GetAll().Result.Where(x => x.ParentId == null);
            if (GeoUnit != null)
            {
              
                List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
                listItemsKyThuat.AddRange(GeoUnit.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsWorkSite = listItemsKyThuat;

            }
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
            return View();
        }

        [AbpMvcAuthorize]
        public async Task<IActionResult> MakeAnAppointment()
        {
            var CatUnitRank = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Cấp bậc")).FirstOrDefault();
            if (CatUnitRank != null)
            {
                var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnitRank.Id).Result;
                List<SelectListItem> listItemsCapBac = new List<SelectListItem>();
                listItemsCapBac.AddRange(CatParent.Items.Select(x => new SelectListItem
                {
                    Text = L(x.DisplayName),
                    Value = x.Id.ToString(),
                }));
                ViewBag.listItemsCapBac = listItemsCapBac;

            }
            return View();
        }

        public async Task<JsonResult> Update(CandidateEditDto input)
        {
            try
            {
                var candidateUpdate = await _candidateAppService.Update(input);
                return Json(candidateUpdate);
            }
            catch (Exception ex)
            {

                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }

        }

        public PartialViewResult ChangePictureModal(long? userId)
        {
            ViewBag.UserId = userId;
            return PartialView("_ChangePictureModal");
        }

        [AbpAuthorize]
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
                    fileInput.FileCategory = "CandidateBL";

                    using (var fileService = _fileServiceFactory.Get())
                    {
                        fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);
                        await _candidateAppService.UpdateCandidateBL(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = recruiterId }, fileInput.FilePath);

                        //fileInput.FileUrl = $"/file/get?c={HttpUtility.UrlEncode(SimpleStringCipher.Instance.Encrypt(fileInput.FileName))}";
                        fileInput.FileUrl = $"/file/get?c={HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath))}";
                        fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);
                        //fileInput.FileUrl =  await fileService.Object.Download(streamContent, fileInput);
                    }

                    results.Files.Add(fileInput);
                }
            }

            return Json(results);
        }
        [AbpMvcAuthorize]
        public async Task<IActionResult> InterviewConfirmationModal(long id)
        {
            ViewBag.Id = id;
            MakeAnAppointmentDto makeAnAppointmentDto = new MakeAnAppointmentDto();
            makeAnAppointmentDto = await _makeAnAppointmentAppService.GetDetail(id);
            return PartialView("InterviewConfirmationModal", makeAnAppointmentDto);
        }
        [AbpMvcAuthorize]
        public async Task<IActionResult> RefuseInterviewModal(long id)
        {
            ViewBag.Id = id;   
            MakeAnAppointmentDto makeAnAppointmentDto = new MakeAnAppointmentDto(); 
            makeAnAppointmentDto = await _makeAnAppointmentAppService.GetDetail(id);    
            return PartialView("RefuseInterviewModal", makeAnAppointmentDto);
        } 
        [AbpMvcAuthorize]
        public async Task<IActionResult> DetailMakeAnAppiontment(long id)
        {
            ViewBag.Id = id;   
            MakeAnAppointmentDto makeAnAppointmentDto = new MakeAnAppointmentDto(); 
            makeAnAppointmentDto = await _makeAnAppointmentAppService.GetDetail(id);    
            return PartialView("DetailMakeAnAppiontment", makeAnAppointmentDto);
        }

        [AbpMvcAuthorize]
        public async Task<JsonResult> UpdateMakeAnApppointment(MakeAnAppointmentDto input)
        {
           var output = await _makeAnAppointmentAppService.UpdateForCandidate(input); 
           return Json(output);
        }


    }
}
