using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.HtmlSanitizer;
using Abp.Localization;
using Abp.Web.Models;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
    [Area("Profile")]
	[AbpMvcAuthorize]
	public class RecruitersController : SaaSControllerBase
	{
		private readonly IRecruiterAppService _recruiterAppService;
		private readonly INVNVRecruiterAppService _NVNVRecruiterAppService;
		private readonly ICatUnitAppService _catUnitAppService;
		private readonly IGeoUnitAppService _geoUnitAppService;
		private readonly ILanguageManager _languageManager;
		private readonly FileServiceFactory _fileServiceFactory;
		private readonly IIntroduceAppService _introduceAppService;
		private readonly IUserTypeAppService _userTypeAppService;
		private readonly IContactAppService _contactAppService;
        private readonly ITradingSessionAccountAppService _tradingSessionAccountAppService;
        private readonly ITradingSessionAppService _tradingSessionAppService;

        public RecruitersController(
			IRecruiterAppService recruiterAppService,
			INVNVRecruiterAppService NVNVRecruiterAppService,
			ICatUnitAppService catUnitAppService,
			 IGeoUnitAppService geoUnitAppService,
			ILanguageManager languageManager,
			FileServiceFactory fileServiceFactory,
			IIntroduceAppService introduceAppService,
			IUserTypeAppService userTypeAppService,
            IContactAppService contactAppService,
             ITradingSessionAccountAppService tradingSessionAccountAppService,
            ITradingSessionAppService tradingSessionAppService
            )
		{
			_recruiterAppService = recruiterAppService;
			_catUnitAppService = catUnitAppService;
			_geoUnitAppService = geoUnitAppService;
			_languageManager = languageManager;
			_fileServiceFactory = fileServiceFactory;
			_NVNVRecruiterAppService = NVNVRecruiterAppService;
			_introduceAppService = introduceAppService;
			_contactAppService = contactAppService;
            _tradingSessionAccountAppService = tradingSessionAccountAppService;
            _tradingSessionAppService = tradingSessionAppService;

        }

		public async Task<IActionResult> Detail(long id)
		{
			var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));
            //var List = _geoUnitAppService.GetAll();
            //if (List.Result.Count() == 0)
            //{
            //    await _geoUnitAppService.BuildDemoGeoAsync();
            //}

			#region Encryption

			#endregion

			var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);

			return View(viewModel);
		}

		[AbpAuthorize]
		[DisableRequestSizeLimit]
		public async Task<JsonResult> UploadFile(IFormFile file, long id)
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
					fileInput.FileCategory = "RecruiterBL";

					using (var fileService = _fileServiceFactory.Get())
					{
						fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);
						//await _recruiterAppService.UpdateRecruiterBL(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = id }, fileInput.FilePath);

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

		public async Task<IActionResult> RecruiterInfo(long id)
		{
			id = await _recruiterAppService.GetCrurrentUserId();

			var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));

			#region Encryption

			#endregion

			List<SelectListItem> listItemsQuyMo = new List<SelectListItem>();

			var ListQuyMo = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Quy mô nhân sự")).FirstOrDefault();
			if (ListQuyMo != null)
			{
				var Parent = _catUnitAppService.GetChildrenCatUnit(ListQuyMo.Id).Result;
				foreach (var QuyMo in Parent.Items)
					listItemsQuyMo.Add(new SelectListItem() { Value = QuyMo.Id.ToString(), Text = QuyMo.DisplayName });
				ViewBag.QuyMo = listItemsQuyMo;
			}


			#region chuyên môn kỹ thuật
			var CatUnit = _catUnitAppService.GetAll().Result.Where(x => x.DisplayName.Equals("Ngành nghề")).FirstOrDefault();
			if (CatUnit != null)
			{
				var CatParent = _catUnitAppService.GetChildrenCatUnit(CatUnit.Id).Result;
				List<SelectListItem> listItemsKyThuat = new List<SelectListItem>();
				listItemsKyThuat.AddRange(CatParent.Items.Select(x => new SelectListItem
				{
					Text = x.DisplayName,
					Value = x.Id.ToString(),
				}));
				ViewBag.listItemsKyThuat = listItemsKyThuat;

			}
			#endregion


			var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);

			#region Show Phone/Email
			viewModel.User.UserName = viewModel.User.UserName.Equals(viewModel.User.EmailAddress) ? viewModel.User.EmailAddress : $"{viewModel.User.UserName} / {viewModel.User.EmailAddress}";
			#endregion

			return View(viewModel);
		}

		//public async Task<JsonResult> Create(RecruiterEditDto model)
		//[HttpPost]
		////[UnitOfWork(IsolationLevel.ReadUncommitted)]
		//[HtmlSanitizer]
		//public async Task<ActionResult> UpdateUserInfo(UserTypeInfoViewModel model)
		//{
		//	try
		//	{
		//		await _userTypeAppService.Update(AbpSession.TenantId.Value, new Authorization.Users.Dto.UserEditDto()
		//		{
		//			Name = model.Name,
		//			PhoneNumber = model.PhoneNumber,
		//			EmailAddress = "unknow@bbk.com"
		//		});
		//		//await _userTypeAppService.UpdateUserAsync(new Mdls.Profile.Authorization.Dto.UpdateUserTypeInput()
		//		//{
		//		//	User = new Mdls.Profile.Authorization.Dto.UserTypeDto()
		//		//	{
		//		//		Name = model.Name,
		//		//		PhoneNumber = model.PhoneNumber
		//		//	}
		//		//}); ;

		//		//return View();
		//	}
		//	catch (UserFriendlyException ex)
		//	{
		//		//ViewBag.UseCaptcha = !model.IsExternalLogin && UseCaptchaOnRegistration();
		//		//ViewBag.ErrorMessage = ex.Message;

		//		//model.PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync();

		//		//return View("Register", model);
		//	}
		//	return View("RecruiterInfo");

		//}

		[HttpPost]
		[HtmlSanitizer]
		public async Task<JsonResult> UpdateUserInfoAjax(UserTypeInfoViewModel model)
		{
			try
			{
				await _userTypeAppService.Update(AbpSession.TenantId.Value, new Authorization.Users.Dto.UserEditDto()
				{
					Name = model.Name,
					PhoneNumber = model.PhoneNumber,
					EmailAddress = "unknow@bbk.com"
				});
				//await _recruiterAppService.Update(model);
				return Json(Ok());
			}
			catch (Exception ex)
			{
				return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
			}
		}

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Create(RecruiterEditDto model)
		{
			try
			{
				await _recruiterAppService.Create(model);
				return Json(model);
			}
			catch (Exception ex)
			{
				return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
			}
		}

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> Update(RecruiterEditDto model)
		{
			try
			{
				await _recruiterAppService.Update(model);
				return Json(model);
			}
			catch (Exception ex)
			{
				return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
			}
		}


		#region view NVNV
		public async Task<ActionResult> NVNVRecruiter()
		{
			#region chuyên môn kỹ thuật
			var CatUnit = ( await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
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

			return View();
		}

		public async Task<ActionResult> NVNVViewDetailRecruiter(long id)
		{
			var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));
			var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);
			return View(viewModel);
		}

		public async Task<ActionResult> NVNVEditRecruiter(long id)
		{

			List<SelectListItem> listItemsQuyMo = new List<SelectListItem>();

			var ListQuyMo = (await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Quy mô nhân sự"));
			if (ListQuyMo != null)
			{
				var Parent = await _catUnitAppService.GetChildrenCatUnit(ListQuyMo.Id);
				foreach (var QuyMo in Parent.Items)
					listItemsQuyMo.Add(new SelectListItem() { Value = QuyMo.Id.ToString(), Text = QuyMo.DisplayName });
				ViewBag.QuyMo = listItemsQuyMo;
			}


			#region chuyên môn kỹ thuật
			var CatUnit =(await _catUnitAppService.GetAll()).FirstOrDefault(x => x.DisplayName.Equals("Ngành nghề"));
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

			var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));
			var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);

			return PartialView(viewModel);
		}

		#endregion


		#region Danh sách giới thiệu của tôi
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

        #endregion

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

        public async Task<JsonResult> GetAllGeo()
        {
            var output = await _geoUnitAppService.GetAll();
            return Json(output);
        }

        public async Task<JsonResult> GetChildrenGeoUnit(long Id)
        {
            var output = await _geoUnitAppService.GetChildrenGeoUnit(Id);
            return Json(output);
        }



    }
}
