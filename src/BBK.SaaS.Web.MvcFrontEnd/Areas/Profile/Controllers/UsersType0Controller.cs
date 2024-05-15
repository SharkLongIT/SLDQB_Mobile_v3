using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType0;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Profile")]
    [AbpMvcAuthorize]
    public class UsersType0Controller : SaaSControllerBase
    {
		private readonly ILanguageManager _languageManager;
		public UsersType0Controller(			
            ILanguageManager languageManager
			)
		{
			_languageManager = languageManager;

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

		public async Task<IActionResult> Detail(long id)
		{
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}
			var viewModel = new RecruiterDetailViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
		}
	}
}
