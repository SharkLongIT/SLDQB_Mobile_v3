using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Profile.Reports;
using BBK.SaaS.Web.Models.RightSidebar;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Views.Shared.Components.LinkWidgets
{
    public partial class LinkWidgetsViewComponent : SaaSViewComponent
    {
        private readonly IReportAppService _CmsCatsAppService;
        public LinkWidgetsViewComponent(IReportAppService CmsCatsAppService) 
        { 
            _CmsCatsAppService = CmsCatsAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cmsCatList = await _CmsCatsAppService.GetAllReportArticle();
            CmsCatViewModel model = new CmsCatViewModel();
            model.Categories = cmsCatList.ReportListArticle;
            return View(model);
        }
    }
}
