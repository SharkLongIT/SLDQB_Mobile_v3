using System.Threading.Tasks;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Widgets;
using BBK.SaaS.Web.Views;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace BBK.SaaS.Web.Views.Shared.Components.Widget;

public partial class WidgetViewComponent : SaaSViewComponent
{
	private readonly IArticlesAppService _articlesAppService;
	private readonly IWidgetZoneAppService _widgetZoneAppService;

	public WidgetViewComponent(IArticlesAppService articlesAppService, IWidgetZoneAppService widgetZoneAppService)
	{
		_articlesAppService = articlesAppService;
		_widgetZoneAppService = widgetZoneAppService;

	}

	public async Task<IViewComponentResult> InvokeAsync(string zone, object additionalData = null)
	{
		//var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

		////no data?
		//if (!model.Any())
		//    return Content("");

		//return View(model);
		//return new HtmlContentViewComponentResult(new HtmlString(script ?? string.Empty));

		WidgetZoneInfo widgetZoneInfo = await _widgetZoneAppService.GetWidgetZoneInfo(zone);

		string htmlContent = string.Empty;
		foreach (var widget in widgetZoneInfo.Widgets)
		{
			htmlContent += widget.HTMLContent;
		}

		if (!string.IsNullOrEmpty(htmlContent))
		{
			return new HtmlContentViewComponentResult(new HtmlString(htmlContent ?? string.Empty));
		}

		return Content("");
	}
}