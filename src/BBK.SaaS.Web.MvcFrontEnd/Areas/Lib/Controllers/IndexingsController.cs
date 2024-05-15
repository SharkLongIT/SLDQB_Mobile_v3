using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.Localization;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Web.Areas.Lib.Models.Indexings;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Lib")]
	[AbpMvcAuthorize]
	public class IndexingsController : SaaSControllerBase
	{
		private readonly ILanguageManager _languageManager;
		private readonly IRepository<CatUnit, long> _catUnitRepository;

		public IndexingsController(
			ILanguageManager languageManager,
			IRepository<CatUnit, long> catUnitRepository
			)
		{
			_catUnitRepository = catUnitRepository;
			_languageManager = languageManager;

		}
		public IActionResult Index()
		{
			//if (baseLanguageName.IsNullOrEmpty())
			//{
			//	baseLanguageName = _languageManager.CurrentLanguage.Name;
			//}
			var viewModel = new CatUnitsViewModel();

			//viewModel.Languages = _languageManager.GetLanguages().ToList();

			return View(viewModel);
		}

		//[AbpMvcAuthorize(AppPermissions.Pages_Administration_CategoryUnits_ManageCategoryTree)]
		public PartialViewResult CreateCategoryUnitModal(long? parentId)
		{
			return PartialView("_CreateCategoryUnitModal", new CreateCategoryUnitModalViewModel(parentId));
		}

		public async Task<PartialViewResult> EditCategoryUnitModal(long id)
		{
			var organizationUnit = await _catUnitRepository.GetAsync(id);
			var model = ObjectMapper.Map<EditCategoryUnitModalViewModel>(organizationUnit);

			return PartialView("_EditCategoryUnitModal", model);
		}
	}
}
