using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.Extensions;
using Abp.MimeTypes;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Dto;
using BBK.SaaS.Storage;
using Abp.Authorization;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using BBK.SaaS.Net;
using Abp.Runtime.Security;
using System.Web;
using BBK.SaaS.Security;
using BBK.SaaS.Mdls.Cms.Widgets;
using Abp.UI;
using Abp.IO.Extensions;
using BBK.SaaS.Graphics;

namespace BBK.SaaS.Web.Controllers
{
    public class FileController : SaaSControllerBase
    {
        private readonly ITempFileCacheManager _tempFileCacheManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly IMimeTypeMap _mimeTypeMap;
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IWidgetZoneManager WidgetZoneManager;
        private readonly IImageValidator _imageValidator;

        public FileController(
            ITempFileCacheManager tempFileCacheManager,
            IBinaryObjectManager binaryObjectManager,
            FileServiceFactory fileServiceFactory,
            IMimeTypeMap mimeTypeMap,
            IWidgetZoneManager WidgetZoneManager,
             IImageValidator imageValidator
        )
        {
            _tempFileCacheManager = tempFileCacheManager;
            _binaryObjectManager = binaryObjectManager;
            _fileServiceFactory = fileServiceFactory;
            _mimeTypeMap = mimeTypeMap;
            this.WidgetZoneManager = WidgetZoneManager;
            _imageValidator = imageValidator;
        }

        [DisableAuditing]
        public ActionResult DownloadTempFile(FileDto file)
        {
            var fileBytes = _tempFileCacheManager.GetFile(file.FileToken);
            if (fileBytes == null)
            {
                return NotFound(L("RequestedFileDoesNotExists"));
            }

            return File(fileBytes, file.FileType, file.FileName);
        }

        [DisableAuditing]
        public async Task<ActionResult> DownloadBinaryFile(Guid id, string contentType, string fileName)
        {
            var fileObject = await _binaryObjectManager.GetOrNullAsync(id);
            if (fileObject == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            if (fileName.IsNullOrEmpty())
            {
                if (!fileObject.Description.IsNullOrEmpty() &&
                    !Path.GetExtension(fileObject.Description).IsNullOrEmpty())
                {
                    fileName = fileObject.Description;
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }
            }

            if (contentType.IsNullOrEmpty())
            {
                if (!Path.GetExtension(fileName).IsNullOrEmpty())
                {
                    contentType = _mimeTypeMap.GetMimeType(fileName);
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest);
                }
            }

            return File(fileObject.Bytes, contentType, fileName);
        }

        //[DisableAuditing]
        [AbpAuthorize]
        [DisableRequestSizeLimit]
        public async Task<JsonResult> MultiUploadFile(List<IFormFile> files)
        {
            //if (!ModelState.IsValid)
            //	throw new UserFriendlyException("ModelState Invalid");

            var results = new FileUploadSummary();
            long size = files.Sum(f => f.Length);
            results.TotalSizeUploaded = size.ToString();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var streamContent = formFile.OpenReadStream())
                    {
                        //string fullFilePath = await _fileService.CreateOrUpdate(new Storage.StorageProviderSaveArgs(filePath, stream));

                        FileMgr fileInput = new FileMgr();
                        fileInput.FileName = formFile.FileName;
                        fileInput.TenantId = AbpSession.TenantId.Value;
                        fileInput.CreatedAt = DateTime.Now;
                        //fileInput.Content = stream;

                        //fileInput = await _fileService.CreateOrUpdate(fileInput);
                        //using (var fileService = await _fileServiceFactory.Get())
                        using (var fileService = _fileServiceFactory.Get())
                        {
                            fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);
                            fileInput.FileUrl = $"/file/get?c={HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath))}";
                            fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);
                        }

                        results.Files.Add(fileInput);
                    }
                }
            }

            return Json(results);
        }

        [AbpAllowAnonymous]
        //[ResponseCache(VaryByHeader = "User-Agent", Duration = 30)]
        public async Task<ActionResult> Get(string c, string p)
        {
            if (!string.IsNullOrEmpty(c) || !string.IsNullOrEmpty(p))
            {
                int tenantId = AbpSession.TenantId ?? 1;
                var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = (!string.IsNullOrEmpty(p)) ? p : StringCipher.Instance.Decrypt(c) };
                //var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = StringCipher.Instance.Decrypt(c) };
                //using (var profileImageService = await _fileServiceFactory.Get())
                using (var profileImageService = _fileServiceFactory.Get())
                {
                    var profileImage = await profileImageService.Object.Download(fileMgr);
                    return File(Convert.FromBase64String(profileImage.ContentString),
                        !string.IsNullOrEmpty(profileImage.FileType) ? profileImage.FileType : MimeTypeNames.ApplicationOctetStream);
                    //return new GetProfilePictureOutput(profileImage);
                }
            }
            return BadRequest();
        }

        public void UploadTempImage(FileDto input)
        {
            var profilePictureFile = Request.Form.Files[0];

            //Check input
            if (profilePictureFile == null)
            {
                throw new UserFriendlyException(L("ProfilePicture_Change_Error"));
            }

            //if (profilePictureFile.Length > MaxProfilePictureSize)
            //{
            //    throw new UserFriendlyException(L("ProfilePicture_Warn_SizeLimit",
            //        AppConsts.MaxProfilePictureBytesUserFriendlyValue));
            //}

            byte[] fileBytes;
            using (var stream = profilePictureFile.OpenReadStream())
            {
                fileBytes = stream.GetAllBytes();
                _imageValidator.Validate(fileBytes); // validate image format
            }

            _tempFileCacheManager.SetFile(input.FileToken, fileBytes);
        }

        public async Task<ActionResult> GetEmailTemplate(string c)
        {
            if (!string.IsNullOrEmpty(c))
            {
                int tenantId = AbpSession.TenantId ?? 1;
                var fileMgr = new FileMgr()
                {
                    TenantId = tenantId,
                    FilePath = "\\emailgioithieu.json",
                    FileCategory = "EmailTemplates"
                };
                //using (var profileImageService = await _fileServiceFactory.Get())
                using (var profileImageService = _fileServiceFactory.Get())
                {
                    var profileImage = await profileImageService.Object.Download(fileMgr);
                    return File(Convert.FromBase64String(profileImage.ContentString),
                        !string.IsNullOrEmpty(profileImage.FileType) ? profileImage.FileType : MimeTypeNames.ApplicationOctetStream);
                    //return new GetProfilePictureOutput(profileImage);
                }
            }
            return BadRequest();
        }

        public async Task<JsonResult> Clear(string zoneName)
        {
            await WidgetZoneManager.ClearWidgetZoneCacheItemAsync(zoneName);
            return Json(new { zoneName });
        }

        //[AbpAllowAnonymous]
        //public async Task<ActionResult> GetPicture(string c)
        //{
        //	if (!string.IsNullOrEmpty(c))
        //	{
        //		int tenantId = AbpSession.TenantId ?? 1;
        //		var fileMgr = new FileMgr() { TenantId = tenantId, FilePath = StringCipher.Instance.Decrypt(c) };
        //		//using (var profileImageService = await _fileServiceFactory.Get())
        //		using (var profileImageService = _fileServiceFactory.Get())
        //		{
        //			var profileImage = await profileImageService.Object.Download(fileMgr);
        //			return File(Convert.FromBase64String(profileImage.ContentString),
        //				!string.IsNullOrEmpty(profileImage.FileType) ? profileImage.FileType : MimeTypeNames.ApplicationOctetStream);
        //			//return new GetProfilePictureOutput(profileImage);
        //		}
        //	}
        //	return BadRequest();
        //}

    }
}
