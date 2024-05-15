using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Profile.TradingSessions;
using BBK.SaaS.Web.Models.Cms.Articles;
using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Views.Shared.Components.HomeCategories;

public partial class NewsWidgetsViewComponent : SaaSViewComponent
{
	private readonly IArticlesAppService _articlesAppService;
	private readonly IArticleFrontEndService _articleFrontEndService;
	private readonly IFECntCategoryAppService _feCntCatService;
	private readonly ITradingSessionAppService _tradingSessionAppService;

	public NewsWidgetsViewComponent(IArticlesAppService articlesAppService, IArticleFrontEndService articleFrontEndService, IFECntCategoryAppService feCntCatService , ITradingSessionAppService tradingSessionAppService)
	{
		_articlesAppService = articlesAppService;
		_articleFrontEndService = articleFrontEndService;
		_feCntCatService = feCntCatService;
		_tradingSessionAppService = tradingSessionAppService;
	}

	public async Task<IViewComponentResult> InvokeAsync(string zone, string type, long? categoryId)
	{
		//if (!string.IsNullOrWhiteSpace(zone) || !string.IsNullOrWhiteSpace(type))
		//{
		//    var cntCats = await _feCntCatService.GetCntCategoriesWithArticles();

		//    ListCntCatsViewModel modelCats = new(cntCats.Items);

		//    return View("ThreeCols", modelCats);
		//}

		//var listArts = await _articlesAppService.GetFENewestArticles();
		var listArts = await _articleFrontEndService.GetArticles(new SearchArticlesInput() { MaxResultCount = 6 });
		//var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);
		var listTrading = await _tradingSessionAppService.GetAllUnitOfWork();
        ////no data?
        //if (!model.Any())
        //    return Content("");
        ListArticlesViewModel model = new(listArts.Items);

		if (!string.IsNullOrEmpty(zone))
		{
			model.ZoneName = zone;
		}

		if (listArts.Items.Count == 0)
		{
			//listArts.Items = new List<ArticleViewDto>() { new ArticleViewDto() { Title = "" } }.ToImmutableList();
			var showNoneItem = new List<ArticleListViewDto>() { new() { Title = "" } };
			model.Articles = showNoneItem;
		}

		model.tradingSessions = listTrading.Items.Take(6).ToList();
		return View(model);

		//return Content($"{widgetZone}: HELLO THERE! ");
	}
}
