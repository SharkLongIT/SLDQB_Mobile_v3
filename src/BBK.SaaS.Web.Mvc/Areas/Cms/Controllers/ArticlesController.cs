using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Authorization;
using Abp.Runtime.Security;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Security;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.Cms.Models.Articles;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BBK.SaaS.Web.Areas.App.Controllers
{
    [Area("Cms")]
    [AbpMvcAuthorize]
    public class ArticlesController : SaaSControllerBase
    {
        private readonly FileServiceFactory _fileServiceFactory;
        private readonly IArticlesAppService _articlesAppService;
        private readonly ICmsCatsAppService _cmsCatsAppService;

        public ArticlesController(
            FileServiceFactory fileServiceFactory,
            ICmsCatsAppService cmsCatsAppService,
            IArticlesAppService articlesAppService
            )
        {
            _fileServiceFactory = fileServiceFactory;
            _cmsCatsAppService = cmsCatsAppService;
            _articlesAppService = articlesAppService;
        }

        public async Task<ActionResult> Index()
        {
            return View();
        }

        public async Task<ActionResult> Create()
        {
            var categories = await _cmsCatsAppService.GetCmsCatsByLevel(new Mdls.Cms.Categories.Dto.GetCmsCatInput());
            CreateArticleViewModel model = new() { Categories = categories.Items };

            ViewBag.FileToken = Guid.NewGuid().ToString();

            return View(model);
        }

        public async Task<ActionResult> Update(long id)
        {
            var categories = await _cmsCatsAppService.GetCmsCatsByLevel(new Mdls.Cms.Categories.Dto.GetCmsCatInput());
            EditArticleViewModel model = new()
            {
                Categories = categories.Items,
                Article = await _articlesAppService.GetArticleForEditAsync(new EntityDto<long> { Id = id }),
            };

            ViewBag.FileToken = Guid.NewGuid().ToString();

            return View(model);
        }

        [DisableRequestSizeLimit]
        public async Task<JsonResult> UploadFile(IFormFile file/*, long recruiterId*/)
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
                    fileInput.FileCategory = "CmsMedia";

                    using (var fileService = _fileServiceFactory.Get())
                    {
                        fileInput = await fileService.Object.CreateOrUpdate(streamContent, fileInput);

                        fileInput.FileUrl = HttpUtility.UrlEncode(StringCipher.Instance.Encrypt(fileInput.FilePath));
                        fileInput.FilePath = StringCipher.Instance.Encrypt(fileInput.FilePath);
                    }

                    results.Files.Add(fileInput);
                }
            }

            return Json(results);
        }
    }
}
