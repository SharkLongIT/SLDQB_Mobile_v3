using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Web.Models.Cms.Articles;
using Microsoft.AspNetCore.Mvc;

namespace BBK.SaaS.Web.Views.Shared.Components.HomeCategories;

public partial class NewsWidgetsViewComponent : SaaSViewComponent
{
    private readonly IArticlesAppService _articlesAppService;
    private readonly IFECntCategoryAppService _feCntCatService;

    public NewsWidgetsViewComponent(IArticlesAppService articlesAppService, IFECntCategoryAppService feCntCatService)
    {
        _articlesAppService = articlesAppService;
        _feCntCatService = feCntCatService;
    }

    public async Task<IViewComponentResult> InvokeAsync(string zone, string type, long? categoryId)
    {
        //if (!string.IsNullOrWhiteSpace(zone) || !string.IsNullOrWhiteSpace(type))
        //{
        //    var cntCats = await _feCntCatService.GetCntCategoriesWithArticles();

        //    ListCntCatsViewModel modelCats = new(cntCats.Items);

        //    return View("ThreeCols", modelCats);
        //}

        var listArts = await _articlesAppService.GetFENewestArticles();
        //var model = await _widgetModelFactory.PrepareRenderWidgetModelAsync(widgetZone, additionalData);

        ////no data?
        //if (!model.Any())
        //    return Content("");
        ListArticlesViewModel model = new(listArts.Items);

        if (listArts.Items.Count == 0)
        {
            //listArts.Items = new List<ArticleViewDto>() { new ArticleViewDto() { Title = "" } }.ToImmutableList();
            var showNoneItem = new List<ArticleListViewDto>(){ new() { Title = "" } };
            model.Articles = showNoneItem;
        }

        return View(model);

        //return Content($"{widgetZone}: HELLO THERE! ");
    }
}
