using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Widgets;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common.Modals;
using BBK.SaaS.Web.Areas.Cms.Models.Widgets;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BBK.SaaS.Web.Areas.Cms.Controllers;

[Area("Cms")]
[AbpMvcAuthorize]
public class WidgetsController : SaaSControllerBase
{
    private readonly IWidgetsAppService _widgetAppService;
    private readonly IRepository<Widget, int> _widgetRepo;
    private readonly IWidgetZoneManager widgetZoneManager;

    public WidgetsController(IWidgetsAppService widgetAppService, IRepository<Widget, int> widgetRepo, IWidgetZoneManager widgetZoneManager)
    {
        _widgetAppService = widgetAppService;
        _widgetRepo = widgetRepo;
        this.widgetZoneManager = widgetZoneManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public PartialViewResult CreateModal()
    {
        return PartialView("_CreateModal");
    }

    public async Task<PartialViewResult> EditModal(EntityDto<int> input)
    {
        var widget = await _widgetRepo.GetAsync(input.Id);
        var model = ObjectMapper.Map<WidgetEditDto>(widget);

        return PartialView("_EditModal", model);
    }

    public IActionResult WidgetZonesCfg()
    {
        ViewBag.PublicZoneNames = typeof(PublicWidgetZones)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(string))
            .Select(property => property.GetValue(null) is string value ? new SelectListItem(value, value) : null)
            .Where(item => item != null)
            .ToList();

        return View();
    }

    public async Task<PartialViewResult> ConfigModal(int? id)
    {
        var output = await _widgetAppService.GetConfigWidgetForEdit(new NullableIdDto<int> { Id = id });
        var viewModel = ObjectMapper.Map<CreateOrEditConfigModalViewModel>(output);

        ViewBag.PublicZoneNames = typeof(PublicWidgetZones)
            .GetProperties(BindingFlags.Public | BindingFlags.Static)
            .Where(property => property.PropertyType == typeof(string))
            .Select(property => property.GetValue(null) is string value ? new SelectListItem(value, value) : null)
            .Where(item => item != null)
            .ToList();

        return PartialView("_ConfigModal", viewModel);
    }

    //public async Task<PartialViewResult> AddWidgetZoneModal(string zoneName)
    //{
    //    if (string.IsNullOrEmpty(zoneName) || !PublicWidgetZones.GetAllZoneNames().Contains(zoneName))
    //    {
    //        throw new UserFriendlyException("ZoneName configuration cannot be empty!");
    //    }

    //    // return existed list of widgets
    //    await widgetZoneManager.GetWidgetsInZoneAsync(zoneName);

    //    //return PartialView("_AddWidgetZoneModal", viewModel);
    //    return PartialView("_AddWidgetZoneModal");
    //}

    public PartialViewResult AddWidgetZoneModal(LookupModalViewModel model)
    {
        return PartialView("_AddWidgetZoneModal", model);
    }

    ///// <summary>
    //   /// Prepare list of available public widget zones
    //   /// </summary>
    //   /// <returns>Available widget zones</returns>
    //   public IList<SelectListItem> PreparePublicWidgetZones()
    //   {
    //       return typeof(PublicWidgetZones)
    //           .GetProperties(BindingFlags.Public | BindingFlags.Static)
    //           .Where(property => property.PropertyType == typeof(string))
    //           .Select(property => property.GetValue(null) is string value ? new SelectListItem(value, value) : null)
    //           .Where(item => item != null)
    //           .ToList();
    //   }
}
