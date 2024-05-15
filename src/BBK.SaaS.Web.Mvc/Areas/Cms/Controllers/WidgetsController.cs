using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Domain.Repositories;
using Abp.UI;
using BBK.SaaS.Configuration;
using BBK.SaaS.Mdls.Cms.Configuration;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Widgets;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;
using BBK.SaaS.Storage;
using BBK.SaaS.Web.Areas.App.Models.Common.Modals;
using BBK.SaaS.Web.Areas.Cms.Models.Widgets;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace BBK.SaaS.Web.Areas.Cms.Controllers;

[Area("Cms")]
[AbpMvcAuthorize]
public class WidgetsController : SaaSControllerBase
{
	private readonly IAppConfigurationAccessor _configurationAccessor;
	private readonly IWidgetsAppService _widgetAppService;
	private readonly IRepository<Widget, int> _widgetRepo;
	private readonly IWidgetZoneManager widgetZoneManager;
	private readonly FileServiceFactory _fileServiceFactory;

	public WidgetsController(IWidgetsAppService widgetAppService, IRepository<Widget, int> widgetRepo, IWidgetZoneManager widgetZoneManager,
		IAppConfigurationAccessor configurationAccessor, FileServiceFactory fileServiceFactory)
	{
		_widgetAppService = widgetAppService;
		_widgetRepo = widgetRepo;
		this.widgetZoneManager = widgetZoneManager;
		_configurationAccessor = configurationAccessor;
		_fileServiceFactory = fileServiceFactory;

	}

	public async Task<IActionResult> Index()
	{
		List<WidgetTemplate> widgettemps = await _widgetAppService.GetWidgetTemplates();

		if (widgettemps != null)
		{
			ViewBag.WidgetTemplates = widgettemps;
		}

		return View();
	}

	public async Task<PartialViewResult> CreateModal(string widgetTemplateName = "")
	{
		if (!string.IsNullOrEmpty(widgetTemplateName))
		{
			//string tempFile = $"e:\\Workspaces\\bbk.saas\\gits5\\BBK.SaaS.SVLQB.Upload\\tenants\\1\\CmsData\\widgettemps_{DateTime.Now:yyyyMMddhhmmss}.json";

			//List<WidgetTemplate> temps = new List<WidgetTemplate>
			//{
			//	new() {
			//		Name = "mofi-title",
			//		Content = "<div class=\"container\"><div class=\"row\"><img src=\"mainimg\" alt=\"maintitle\" class=\"img-fluid\"></div>\r\n</div>",
			//		PlaceHolderStr = "",
			//		Fields = new List<WidgetField>()
			//		{
			//			new() { Name = "maintitle", Title = "Tiêu đề", FieldType = WidgetFieldTypeEnum.text },
			//			new() { Name = "mainimg", Title = "Đường dẫn ảnh đại diện", FieldType = WidgetFieldTypeEnum.text }
			//		}
			//	}
			//};

			//System.IO.File.WriteAllText(tempFile, JsonConvert.SerializeObject(temps));

			//List<WidgetTemplate> widgettemps = null;
			//using (var fileService = _fileServiceFactory.Get())
			//{
			//	//var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
			//	var fileMgr = await fileService.Object.GetFileAsync(new FileInputDto() { TenantId = AbpSession.TenantId.Value, FilePath = "CmsData\\widgettemps.json" });
			//	widgettemps = JsonConvert.DeserializeObject<List<WidgetTemplate>>(Encoding.UTF8.GetString(fileMgr.Content));
			//}

			WidgetTemplate widgettemp = await _widgetAppService.GetWidgetTemplate(widgetTemplateName);

			if (widgettemp != null)
			{
				ViewBag.WidgetTemplate = widgettemp;
			}

			//return PartialView("_CreateWidgetImageModal");
			return PartialView("_CreateByTemplateModal");
		}
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

		if (!string.IsNullOrEmpty(_configurationAccessor.Configuration["App:WebSiteFrontEndAddress"]))
		{
			ViewBag.SyncUrlWebHook = _configurationAccessor.Configuration["App:WebSiteFrontEndAddress"];
		}

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


