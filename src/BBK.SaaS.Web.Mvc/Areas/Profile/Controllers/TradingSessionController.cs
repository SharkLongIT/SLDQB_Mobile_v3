using Abp.AspNetCore.Mvc.Authorization;
using BBK.SaaS.Graphics;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Cms.Controllers;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using BBK.SaaS.Mdls.Cms.Medias;
using Abp.IO.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using BBK.SaaS.Mdls.Category.Geographies;

namespace BBK.SaaS.Web.Areas.Profile.Controllers
{
    [Area("Profile")]
    [AbpMvcAuthorize]
    public class TradingSessionController : SaaSControllerBase
    {
        private readonly ITradingSessionAppService _tradingSessionAppService;
        private readonly MediaTypeManager MediaTypeManager;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IMediasMgrAppService _mediasMgrAppService;
        private readonly IImageValidator _imageValidator;
        private readonly IGeoUnitAppService _geoUnitAppService;
        public TradingSessionController(ITradingSessionAppService tradingSessionAppService, 
            MediaTypeManager mediaTypeManager, 
            FileServiceFactory fileServiceFactory,
            IMediasMgrAppService mediasMgrAppService
            , IImageValidator imageValidator,
            IGeoUnitAppService geoUnitAppService)
        {
            _tradingSessionAppService = tradingSessionAppService;   
            MediaTypeManager = mediaTypeManager;
            _fileServiceFactory = fileServiceFactory;
            _mediasMgrAppService = mediasMgrAppService;
            _imageValidator = imageValidator;
            _geoUnitAppService = geoUnitAppService;
        }

        public async Task<IActionResult> Index()
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
            return View();
        }

        public IActionResult CreateTrading()
        {
            return PartialView();
        }

        public IActionResult EditTrading(long? Id)
        {
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            var GetTrading = _tradingSessionAppService.GetAll(tradingSessionSearch).Result.Items.Where(x=>x.Id == Id).FirstOrDefault();
            TradingSessionEditDto tradingSessionEditDto = new TradingSessionEditDto();
            if (GetTrading != null)
            {
                tradingSessionEditDto.Id = GetTrading.Id;
                tradingSessionEditDto.NameTrading = GetTrading.NameTrading;
                tradingSessionEditDto.ProvinceId = GetTrading.ProvinceId;
                tradingSessionEditDto.DistrictId = GetTrading.DistrictId;
                tradingSessionEditDto.VillageId = GetTrading.VillageId;
                tradingSessionEditDto.Address = GetTrading.Address;
                tradingSessionEditDto.StartTime = GetTrading.StartTime;
                tradingSessionEditDto.EndTime = GetTrading.EndTime;
                tradingSessionEditDto.Description = GetTrading.Description;
                tradingSessionEditDto.CountCandidateMax = GetTrading.CountCandidateMax;
                tradingSessionEditDto.CountRecruiterMax = GetTrading.CountRecruiterMax;
                tradingSessionEditDto.Describe = GetTrading.Describe;
                tradingSessionEditDto.ImgUrl = GetTrading.ImgUrl;
            }
            ViewBag.FileToken = Guid.NewGuid().ToString();
            return PartialView(tradingSessionEditDto);
        }

        public IActionResult ViewTrading(long? Id)
        {
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            var GetTrading = _tradingSessionAppService.GetAll(tradingSessionSearch).Result.Items.Where(x => x.Id == Id).FirstOrDefault();
            TradingSessionEditDto tradingSessionEditDto = new TradingSessionEditDto();
            if (GetTrading != null)
            {
                tradingSessionEditDto.Id = GetTrading.Id;
                tradingSessionEditDto.NameTrading = GetTrading.NameTrading;
                tradingSessionEditDto.ProvinceId = GetTrading.ProvinceId;
                tradingSessionEditDto.DistrictId = GetTrading.DistrictId;
                tradingSessionEditDto.VillageId = GetTrading.VillageId;
                tradingSessionEditDto.Address = GetTrading.Address;
                tradingSessionEditDto.StartTime = GetTrading.StartTime;
                tradingSessionEditDto.EndTime = GetTrading.EndTime;
                tradingSessionEditDto.Description = GetTrading.Description;
                tradingSessionEditDto.CountCandidateMax = GetTrading.CountCandidateMax;
                tradingSessionEditDto.CountRecruiterMax = GetTrading.CountRecruiterMax;
                tradingSessionEditDto.Describe = GetTrading.Describe;
                tradingSessionEditDto.ImgUrl = GetTrading.ImgUrl;
            }
            return PartialView(tradingSessionEditDto);
        }

        //model NTD
        public IActionResult Recruiter(long? Id)
        {
            ViewBag.TradingId = Id;
            return PartialView();
        }

        public IActionResult Candidate(long? Id)
        {
            ViewBag.TradingId = Id;
            return PartialView();
        }


        public async Task<IActionResult> TradingAccount(long? Id)
        {
            ViewBag.Id = Id;
            TradingSessionSearch tradingSessionSearch = new TradingSessionSearch();
            var GetTrading = _tradingSessionAppService.GetAll(tradingSessionSearch).Result.Items.Where(x => x.Id == Id).FirstOrDefault();

            ViewBag.StartTime = GetTrading.StartTime;
            ViewBag.EndTime = GetTrading.EndTime;

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



        [HttpPost]
        public async Task<IActionResult> UploadCroppedPrimaryImage([FromForm] PrimaryImageUploadModel model)
        {
            //if (!ModelState.IsValid)
            //	throw new UserFriendlyException("ModelState Invalid");

            if (string.IsNullOrEmpty(model.FileName))
            {
                model.FileName = "TradingImg";
            }

            // Allow for dropzone uploads
            if (!model.Uploads.Any())
            {
                model.Uploads = HttpContext.Request.Form.Files;
            }

            var results = new FileUploadSummary();
            long size = model.Uploads.Sum(f => f.Length);
            results.TotalSizeUploaded = size.ToString();

            foreach (var formFile in model.Uploads)
            {
                if (!MediaTypeManager.IsSupported(formFile.FileName))
                {
                    continue;
                }

                var mediaType = MediaTypeManager.GetMediaType(formFile.FileName);

                if (formFile.Length > 0 && !string.IsNullOrWhiteSpace(formFile.ContentType))
                {
                    using (var streamContent = formFile.OpenReadStream())
                    {
                        FileInputDto fileInput = new()
                        {
                            FileName = formFile.FileName,
                            TenantId = AbpSession.TenantId.Value,
                            CreatedAt = DateTime.Now,
                            FileCategory = "TradingSession",
                            IsUniqueFileName = false,
                            IsUniqueFolder = false,
                        };
                        FileMgr fileMgr = new FileMgr();

                        // original bytes
                        var fileContent = streamContent.GetAllBytes();

                        using (var fileService = _fileServiceFactory.Get())
                        {
                            if (mediaType == MediaType.Image)
                            {
                                using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent, 800, 640)))
                                {
                                    fileMgr = await fileService.Object.CreateOrUpdate(fileContent, fileInput);
                                }
                            }
                        }

                        return Ok(fileMgr);
                        //results.Files.Add(fileInput);
                    }
                }
            }

            return Ok(results);
            //return Json(results);
        }
    }
}
