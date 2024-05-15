using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Web.Areas.Cms.Models.Categories;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Areas.Cms.Controllers
{
	[Area("Cms")]
	[AbpMvcAuthorize]
	public class CategoriesController : SaaSControllerBase
	{
		private readonly IRepository<CmsCat, long> _cmsCatRepository;
        private readonly ICmsCatsAppService _cmsCatsAppService;

		public CategoriesController(IRepository<CmsCat, long> cmsCatRepository, ICmsCatsAppService cmsCatsAppService)
		{
			_cmsCatRepository = cmsCatRepository;
            _cmsCatsAppService = cmsCatsAppService;
		}

		public ActionResult Index()
        {
            return View();
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        public async Task<PartialViewResult> CreateModal(long? parentId)
        {
            var cmsCategories = await _cmsCatsAppService.GetCmsCats();

			CreateCategoryModalViewModel model = new(parentId)
			{
				Categories = cmsCategories.Items
			};

			return PartialView("_CreateModal", model);
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        public async Task<PartialViewResult> EditModal(long id)
        {
            var cmsCategories = await _cmsCatsAppService.GetCmsCats();
            
            var category = await _cmsCatRepository.GetAsync(id);
            var model = ObjectMapper.Map<EditCategoryModalViewModel>(category);
            model.Categories = cmsCategories.Items;


            return PartialView("_EditModal", model);
        }

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public PartialViewResult AddMemberModal(LookupModalViewModel model)
        //{
        //    return PartialView("_AddMemberModal", model);
        //}

        //[AbpMvcAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageRoles)]
        //public PartialViewResult AddRoleModal(LookupModalViewModel model)
        //{
        //    return PartialView("_AddRoleModal", model);
        //}
    }
}
