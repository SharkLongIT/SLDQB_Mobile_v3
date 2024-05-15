using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Localization;
using BBK.SaaS.Authorization;
using BBK.SaaS.Mdls.Cms.Topics;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Cms")]
	[AbpMvcAuthorize]
	public class TopicsController : SaaSControllerBase
	{
		private readonly ILanguageManager _languageManager;
		private readonly ITopicAppService _topicAppService;
		private readonly FileServiceFactory _fileServiceFactory;

		public TopicsController(
			ILanguageManager languageManager
			, ITopicAppService topicAppService
			, FileServiceFactory fileServiceFactory
			)
		{
			_languageManager = languageManager;
			_topicAppService = topicAppService;
			_fileServiceFactory = fileServiceFactory;

		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Create()
		{
			return View();
		}

		[AbpAuthorize(AppPermissions.Pages_Administration_CommFuncs_Edit)]
		public async Task<ActionResult> Update(long id)
		{
			var dto = await _topicAppService.GetTopicForEditAsync(id);
			ViewBag.DTO = dto;
			return View();
		}

		//[AbpAuthorize]
		//[DisableRequestSizeLimit]
		//public async Task<JsonResult> UploadFile(IFormFile file, long recruiterId)
		//{
		//	//if (!ModelState.IsValid)
		//	//	throw new UserFriendlyException("ModelState Invalid");

		//	var results = new FileUploadSummary();

		//	if (file != null && file.Length > 0)
		//	{
		//		using (var streamContent = file.OpenReadStream())
		//		{
		//			FileMgr fileInput = new FileMgr();
		//			fileInput.FileName = file.FileName;
		//			fileInput.TenantId = AbpSession.TenantId.Value;
		//			fileInput.CreatedAt = DateTime.Now;
		//			fileInput.FileCategory = "RecruiterBL";

		//			using (var fileService = _fileServiceFactory.Get())
		//			{
		//				fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);
		//			}

		//			results.Files.Add(fileInput);
		//		}
		//	}

		//	return Json(results);
		//}
	}
}
