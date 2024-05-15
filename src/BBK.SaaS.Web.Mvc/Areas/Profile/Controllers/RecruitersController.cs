using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Localization;
using Abp.Runtime.Security;
using Abp.Timing;
using Abp.UI;
using Abp.Web.Models;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
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

        public RecruitersController(
            IRecruiterAppService recruiterAppService,
            INVNVRecruiterAppService NVNVRecruiterAppService,
            ICatUnitAppService catUnitAppService,
             IGeoUnitAppService geoUnitAppService,
            ILanguageManager languageManager,
            FileServiceFactory fileServiceFactory
            )
        {
            _recruiterAppService = recruiterAppService;
            _catUnitAppService = catUnitAppService;
            _geoUnitAppService = geoUnitAppService;
            _languageManager = languageManager;
            _fileServiceFactory = fileServiceFactory;
            _NVNVRecruiterAppService = NVNVRecruiterAppService;
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
            //if (id == await _recruiterAppService.GetCrurrentUserId())
            //         {
            //             id = await _recruiterAppService.GetCrurrentUserId();

            //         }

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

            return View(viewModel);
        }



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

		public async Task AddGeoUnit()
        {
            var List = await  _geoUnitAppService.GetAll();
            if (List.Count() == 0)
            {
                 await _geoUnitAppService.BuildDemoGeoAsync();
            }
        }

		public async Task AddCatUnit()
        {
            var List = await _catUnitAppService.GetAll();
            if (List.Count() == 0)
            {
                await _catUnitAppService.BuildDemoCatAsync();
            }
        }

        #region view NVNV
        public async Task<ActionResult> NVNVRecruiter()
        {
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


            #region tinh thanh
            var List = _geoUnitAppService.GetAll().Result.Where(x => x.ParentId == null);
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

            var recruiterDto = await _recruiterAppService.GetRecruiterForEdit(new Abp.Application.Services.Dto.NullableIdDto<long>(id));
            var viewModel = ObjectMapper.Map<RecruiterDetailViewModel>(recruiterDto);

            return PartialView(viewModel);
        }

        #endregion


    }
}
