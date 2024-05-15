using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Web.Areas.Lib.Models.GeoUnits;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Lib")]
	[AbpMvcAuthorize]
	public class GeoUnitsController : SaaSControllerBase
	{
		private readonly ILanguageManager _languageManager;
		public GeoUnitsController(
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
			var viewModel = new GeoUnitsViewModel();

			viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
		}
	}
}
