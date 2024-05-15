using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.IO.Extensions;
using Abp.Localization;
using BBK.SaaS.Graphics;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Medias;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.App.Models.MediaFolders;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BBK.SaaS.Web.Areas.Cms.Controllers
{
    [Area("Cms")]
    [AbpMvcAuthorize]
    public class MediasMgrController : SaaSControllerBase
    {
        private readonly ILanguageManager _languageManager;
        //private readonly IRepository<MediaFolder, long> _mediaFolderRepository;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IMediasMgrAppService _mediasMgrAppService;
        private readonly IImageValidator _imageValidator;
        private readonly MediaTypeManager MediaTypeManager;

        public MediasMgrController(
            ILanguageManager languageManager
            //, IRepository<MediaFolder, long> mediaFolderRepository
            , FileServiceFactory fileServiceFactory
            , IMediasMgrAppService mediasMgrAppService
            , IImageValidator imageValidator, MediaTypeManager mediaTypeManager
            )
        {
            _languageManager = languageManager;
            //_mediaFolderRepository = mediaFolderRepository;
            _fileServiceFactory = fileServiceFactory;
            _mediasMgrAppService = mediasMgrAppService;
            _imageValidator = imageValidator;
            MediaTypeManager = mediaTypeManager;
        }

        public IActionResult Index()
        {
            //if (baseLanguageName.IsNullOrEmpty())
            //{
            //	baseLanguageName = _languageManager.CurrentLanguage.Name;
            //}
            //var viewModel = new MediaFoldersViewModel();

            //viewModel.Languages = _languageManager.GetLanguages().ToList();

            //return View(viewModel);

            return View();
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        public PartialViewResult CreateMediaFolderModal(long? parentId)
        {
            return PartialView("_CreateMediaFolderModal", new CreateMediaFolderModalViewModel(parentId));
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        //public async Task<PartialViewResult> EditModal(long id)
        //{
        //    var organizationUnit = await _organizationUnitRepository.GetAsync(id);
        //    var model = ObjectMapper.Map<EditOrganizationUnitModalViewModel>(organizationUnit);

        //    return PartialView("_EditModal", model);
        //}

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles)]
        //public PartialViewResult AddRoleModal(LookupModalViewModel model)
        //{
        //    return PartialView("_AddRoleModal", model);
        //}

        public PartialViewResult InsertMediaModal()
        {
            return PartialView("_InsertMediaModal");
        }

        public PartialViewResult CropPrimaryImageModal(string token, string c)
        {
            ViewBag.PrimaryImageUrl = c;
            ViewBag.FileToken = token;
            return PartialView("_CropPrimaryImageModal");
        }

        [HttpPost]
        public async Task<IActionResult> UploadCroppedPrimaryImage([FromForm] PrimaryImageUploadModel model)
        {
            //if (!ModelState.IsValid)
            //	throw new UserFriendlyException("ModelState Invalid");

            if (string.IsNullOrEmpty(model.FileName))
            {
                model.FileName = "ArticleImg";
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
                            FileCategory = "CmsArticles",
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

        [DisableRequestSizeLimit]
        //[Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromForm] MediaUploadModel model)
        {
            //if (!ModelState.IsValid)
            //	throw new UserFriendlyException("ModelState Invalid");

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
                        FileMgr fileInput = new()
                        {
                            FileName = formFile.FileName,
                            TenantId = AbpSession.TenantId.Value,
                            CreatedAt = DateTime.Now,
                            FileCategory = "CmsMedia"
                        };

                        // original bytes
                        var fileContent = streamContent.GetAllBytes();

                        using (var fileService = _fileServiceFactory.Get())
                        {
                            if (mediaType == MediaType.Image)
                            {
                                using (var outputStream = new MemoryStream(_imageValidator.MakeWebp(fileContent)))
                                {
                                    fileInput = await fileService.Object.CreateOrUpdateImage(outputStream, fileInput);
                                }

                                await _mediasMgrAppService.SaveMediaAsync(new Mdls.Cms.Medias.Dto.MediaDto()
                                {
                                    //ContentType = formFile.ContentType,
                                    ContentType = MimeTypes.GetMimeType("*.webp"),
                                    Filename = formFile.FileName,
                                    FolderId = model.FolderId,
                                    Size = formFile.Length.ToString(),
                                    Type = MediaTypeManager.GetMediaType(formFile.FileName), //Mdls.Cms.MediaType.Image,
                                    PublicUrl = fileInput.FilePath
                                });

                                using (var outputStream = new MemoryStream(_imageValidator.MakeThumbnail(fileContent, 210, 160)))
                                {
                                    await fileService.Object.SaveImageThumbnail(outputStream, fileInput);
                                }
                            }
                            else
                            {
                                using var outputStream = new MemoryStream(fileContent);
                                fileInput = await fileService.Object.CreateOrUpdate(outputStream, fileInput);

                                await _mediasMgrAppService.SaveMediaAsync(new Mdls.Cms.Medias.Dto.MediaDto()
                                {
                                    ContentType = MimeTypes.GetMimeType(formFile.FileName),
                                    Filename = formFile.FileName,
                                    FolderId = model.FolderId,
                                    Size = formFile.Length.ToString(),
                                    Type = MediaTypeManager.GetMediaType(formFile.FileName), //Mdls.Cms.MediaType.Image,
                                    PublicUrl = fileInput.FilePath
                                });
                            }
                        }

                        results.Files.Add(fileInput);
                    }
                }
            }

            return Ok(results);
            //return Json(results);
        }
    }

    public class MediaUploadModel
    {
        /// <summary>
        /// Gets/sets the Folder id.
        /// </summary>
        public int? FolderId { get; set; }

        /// <summary>
        /// Gets/sets the uploaded file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFormFile> Uploads { get; set; } = new List<IFormFile>();
    }

    public class PrimaryImageUploadModel
    {
        /// <summary>
        /// Gets/sets the Folder id.
        /// </summary>
        public long? ArticleId { get; set; }

        public string FileName { get; set; }

        /// <summary>
        /// Gets/sets the uploaded file.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IFormFile> Uploads { get; set; } = new List<IFormFile>();
    }

}
