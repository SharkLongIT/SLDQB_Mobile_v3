using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.HtmlSanitizer;
using Abp.Localization;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IIntroduceAppService _introduceAppService;
        private readonly IContactAppService _contactAppService;
        private readonly ITradingSessionAccountAppService _tradingSessionAccountAppService;
        private readonly ITradingSessionAppService _tradingSessionAppService;

        public CandidateController(
            ICandidateAppService candidateAppService,
            ILanguageManager languageManager,
            FileServiceFactory fileServiceFactory,
            IJobApplicationAppService jobApplicationAppService,
            ICatUnitAppService catUnitAppService,
            IGeoUnitAppService geoUnitAppService,
            IMakeAnAppointmentAppService makeAnAppointmentAppService,
            IIntroduceAppService introduceAppService,
            IContactAppService contactAppService,
            ITradingSessionAccountAppService tradingSessionAccountAppService,
            ITradingSessionAppService tradingSessionAppService
            )
        {
            _candidateAppService = candidateAppService;
            _languageManager = languageManager;
            _fileServiceFactory = fileServiceFactory;
            _jobApplicationAppService = jobApplicationAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
            _makeAnAppointmentAppService = makeAnAppointmentAppService;
            _introduceAppService = introduceAppService;
            _contactAppService = contactAppService;
            _tradingSessionAccountAppService = tradingSessionAccountAppService;
            _tradingSessionAppService = tradingSessionAppService;
        }

        [AbpMvcAuthorize]
        public async Task<IActionResult> Detail(long id)
        {
            if (id == 0)
            {
                id = AbpSession.UserId.Value;
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

            #region Show Phone/Email
            viewModel.User.UserName = viewModel.User.UserName.Equals(viewModel.User.EmailAddress) ? viewModel.User.EmailAddress : $"{viewModel.User.UserName} / {viewModel.User.EmailAddress}";
            #endregion

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
            var CatUnitRank = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Cấp bậc"));

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
        [HttpPost]
        [HtmlSanitizer]
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
        [AbpMvcAuthorize]
        public async Task<JsonResult> getAllByUserType(IntroduceSearch input)
        {
            var output = await _introduceAppService.GetAllByUserType(input);
            return Json(output);
        }
        [AbpMvcAuthorize]
        public async Task<JsonResult> GetAllOfCandidate(MakeAnAppointmentInput input)
        {
            var output = await _makeAnAppointmentAppService.GetAllOfCandidate(input);
            return Json(output);
        }

        [AbpMvcAuthorize]
        public IActionResult Introduce()
        {
            return View("Introduce");
        }
        public async Task<IActionResult> DetailIntroduce(long Id)
        {
            IntroduceSearch introduceSearch = new IntroduceSearch();
            var InFo = _introduceAppService.GetAll(introduceSearch).Result.Items.Where(x => x.Id == Id).FirstOrDefault();
            var model = new IntroduceEditDto();
            model.Id = Id;
            model.FullName = InFo.FullName;
            model.Email = InFo.Email;
            model.Phone = InFo.Phone;
            model.Description = InFo.Description;
            model.Status = InFo.Status;
            model.ArticleId = InFo.ArticleId;

            return PartialView("DetailIntroduce", model);
        }

        #region Danh sách câu hỏi của tôi
        [AbpMvcAuthorize]
        public IActionResult QuestionsOfMe()
        {
            return View("QuestionsOfMe");
        }

        [AbpMvcAuthorize]
        public async Task<JsonResult> GetAllQuestionsOfCandidate(ContactSearch input)
        {
            var output = await _contactAppService.GetAllOfMe(input);
            return Json(output);
        }

        [AbpMvcAuthorize]
        public async Task<IActionResult> QuestionsOfMeDetail(long Id)
        {
            var contact = await _contactAppService.GetById(new NullableIdDto<long> { Id = Id });
            //return Json(output);

            return PartialView("QuestionsOfMeDetail", contact);
        }
        #endregion
        
        #region Danh sách phiên giao dich đã tham gia
        [AbpMvcAuthorize]
        public IActionResult TradingSessionsOfMe()
        {
            return View("TradingSessionsOfMe");
        }

        [AbpMvcAuthorize]
        public async Task<JsonResult> GetAllTradingSessionsOfMe(TradingSessionSearch input)
        {
            var output = await _tradingSessionAccountAppService.GetAllByUserId(input);

            return Json(output);
        }
        [AbpMvcAuthorize]
        public async Task<JsonResult> UpdateStatusTradingSession(TradingSessionAccountEditDto input)
        {
            await _tradingSessionAccountAppService.UpdateStatus(input);

            return Json(Ok());
        }

        #endregion 

        public async Task GenPDF(long JobId , int IdTemplate)
        {
            try
            {
                await _candidateAppService.GeneratePdf(JobId, IdTemplate);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
