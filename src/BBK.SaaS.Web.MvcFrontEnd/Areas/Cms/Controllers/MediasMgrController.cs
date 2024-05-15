//using Abp.AspNetCore.Mvc.Authorization;
//using Abp.Domain.Repositories;
//using Abp.Localization;
//using BBK.SaaS.Graphics;
//using BBK.SaaS.Mdls.Cms.Entities;
//using BBK.SaaS.Mdls.Cms.Medias;
//using BBK.SaaS.Security;
//using BBK.SaaS.Storage;
//using BBK.SaaS.Web.Areas.App.Models.MediaFolders;
//using BBK.SaaS.Web.Controllers;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace BBK.SaaS.Web.Areas.Cms.Controllers
//{
//	[Area("Cms")]
//	[AbpMvcAuthorize]
//	public class MediasMgrController : SaaSControllerBase
//	{
//		private readonly ILanguageManager _languageManager;
//		//private readonly IRepository<MediaFolder, long> _mediaFolderRepository;
//		private readonly FileServiceFactory _fileServiceFactory;
//		private readonly IMediasMgrAppService _mediasMgrAppService;
//        private readonly IImageValidator _imageValidator;

//		public MediasMgrController(
//			ILanguageManager languageManager
//			//, IRepository<MediaFolder, long> mediaFolderRepository
//			, FileServiceFactory fileServiceFactory
//			, IMediasMgrAppService mediasMgrAppService
//			, IImageValidator imageValidator
//			)
//		{
//			_languageManager = languageManager;
//			//_mediaFolderRepository = mediaFolderRepository;
//			_fileServiceFactory = fileServiceFactory;
//			_mediasMgrAppService = mediasMgrAppService;
//			_imageValidator = imageValidator;

//		}
//		public IActionResult Index()
//		{
//			//if (baseLanguageName.IsNullOrEmpty())
//			//{
//			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
//			//}
//			//var viewModel = new MediaFoldersViewModel();

//			//viewModel.Languages = _languageManager.GetLanguages().ToList();

//			//return View(viewModel);

//			return View();
//		}

//		//[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
//		public PartialViewResult CreateMediaFolderModal(long? parentId)
//		{
//			return PartialView("_CreateMediaFolderModal", new CreateMediaFolderModalViewModel(parentId));
//		}

//		//[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
//		//public async Task<PartialViewResult> EditModal(long id)
//		//{
//		//    var organizationUnit = await _organizationUnitRepository.GetAsync(id);
//		//    var model = ObjectMapper.Map<EditOrganizationUnitModalViewModel>(organizationUnit);

//		//    return PartialView("_EditModal", model);
//		//}

//		//[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles)]
//		//public PartialViewResult AddRoleModal(LookupModalViewModel model)
//		//{
//		//    return PartialView("_AddRoleModal", model);
//		//}

//		public PartialViewResult InsertMediaModal()
//		{
//			return PartialView("_InsertMediaModal");
//		}

//		[DisableRequestSizeLimit]
//		//[Consumes("multipart/form-data")]
//		[HttpPost]
//		public async Task<IActionResult> UploadFiles([FromForm] MediaUploadModel model)
//		{
//			//if (!ModelState.IsValid)
//			//	throw new UserFriendlyException("ModelState Invalid");

//			// Allow for dropzone uploads
//			if (!model.Uploads.Any())
//			{
//				model.Uploads = HttpContext.Request.Form.Files;
//			}

//			var results = new FileUploadSummary();
//			long size = model.Uploads.Sum(f => f.Length);
//			results.TotalSizeUploaded = size.ToString();

//			foreach (var formFile in model.Uploads)
//			{
//				if (formFile.Length > 0 && !string.IsNullOrWhiteSpace(formFile.ContentType))
//				{
//					using (var streamContent = formFile.OpenReadStream())
//					{
//						//string fullFilePath = await _fileService.CreateOrUpdate(new Storage.StorageProviderSaveArgs(filePath, stream));

//						//await _api.Media.SaveAsync(new StreamMediaContent
//						//{
//						//	Id = model.Uploads.Count() == 1 ? model.Id : null,
//						//	FolderId = model.ParentId,
//						//	Filename = Path.GetFileName(upload.FileName),
//						//	Data = stream
//						//});



//						FileMgr fileInput = new()
//						{
//							FileName = formFile.FileName,
//							TenantId = AbpSession.TenantId.Value,
//							CreatedAt = DateTime.Now,
//							FileCategory = "CmsMedia"

//						};

//						using (var fileService = _fileServiceFactory.Get())
//						{
//							fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);
//							//fileInput.FileUrl = $"/file/get?c={HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath))}";

//							await _mediasMgrAppService.SaveMediaAsync(new Mdls.Cms.Medias.Dto.MediaDto()
//							{
//								ContentType = formFile.ContentType,
//								Filename = formFile.FileName,
//								FolderId = model.FolderId,
//								Size = formFile.Length.ToString(),
//								Type = Mdls.Cms.MediaType.Image,
//								PublicUrl = fileInput.FilePath
//							});

//							streamContent.Position = 0;

//							using (var outputStream = new MemoryStream(_imageValidator.MakeThumbnail(streamContent, 210, 160)))
//							{
								
//								FileMgr thumbs = new()
//								{
//									FileName = fileInput.FileName,
//									TenantId = AbpSession.TenantId.Value,
//									CreatedAt = DateTime.Now,
//									FileCategory = "CmsMedia"
//								};

//								await fileService.Object.SaveImageThumbnail(outputStream, thumbs);
//							}
								
//							//fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);
//						}

//						results.Files.Add(fileInput);
//					}
//				}
//			}

//			return Ok(results);

//			//return Json(results);
//		}
//	}

//	public class MediaUploadModel
//	{
//		/// <summary>
//		/// Gets/sets the Folder id.
//		/// </summary>
//		public int? FolderId { get; set; }

//		/// <summary>
//		/// Gets/sets the uploaded file.
//		/// </summary>
//		/// <returns></returns>
//		public IEnumerable<IFormFile> Uploads { get; set; } = new List<IFormFile>();
//	}

//}
