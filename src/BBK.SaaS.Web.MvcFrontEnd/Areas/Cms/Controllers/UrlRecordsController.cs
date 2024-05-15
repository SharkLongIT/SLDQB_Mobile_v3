using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Mdls.Cms.Caching;
using BBK.SaaS.Mdls.Cms.UrlRecords;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType0;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Cms")]
    [AbpMvcAuthorize]
    public class UrlRecordsController : SaaSControllerBase
    {
		private readonly ILanguageManager _languageManager;
		//private readonly ISlugCache _slugCache;
		private readonly ICmsRedisCacheManager _cmsRedisCacheManager;

		public UrlRecordsController(			
            ILanguageManager languageManager,
			ICmsRedisCacheManager cmsRedisCacheManager
			)
		{
			_languageManager = languageManager;
			_cmsRedisCacheManager = cmsRedisCacheManager;
		}
		public IActionResult Index()
        {
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}
			var viewModel = new UsersType0ViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
        }

		public async Task<IActionResult> RedisKeys(long id)
		{
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}

			await ((ICmsRedisCache)_cmsRedisCacheManager.GetCache("ViewedCount")).GetKeys("");

			var viewModel = new RecruiterDetailViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View("RedisKeys", viewModel);
		}
	}
}
