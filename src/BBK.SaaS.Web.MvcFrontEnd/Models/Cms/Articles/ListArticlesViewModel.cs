using System.Collections.Generic;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;

namespace BBK.SaaS.Web.Models.Cms.Articles
{
    public class ListArticlesViewModel
    {
        public string ZoneName { get; set; }
        public IReadOnlyList<ArticleListViewDto> Articles { get; set; }
        public List<TradingSessionEditDto> tradingSessions { get; set; }
        public ListArticlesViewModel(IReadOnlyList<ArticleListViewDto> articles)
        {
            Articles = articles;
        }

        public ListArticlesViewModel()
        {
            Articles = new List<ArticleListViewDto>();
        }
    }

    public class ListCntCatsViewModel
    {
        public IReadOnlyList<ContentCategoryDto> CntCategories { get; set; }

        public ListCntCatsViewModel(IReadOnlyList<ContentCategoryDto> articles)
        {
            CntCategories = articles;
        }

        public ListCntCatsViewModel()
        {
            CntCategories = new List<ContentCategoryDto>();
        }
    }
}
