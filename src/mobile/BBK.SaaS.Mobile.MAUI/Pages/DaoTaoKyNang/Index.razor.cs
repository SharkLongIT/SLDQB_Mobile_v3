﻿using Abp.Threading;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.DaoTaoKyNang
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected IArticlesAppService articlesAppService { get; set; }
        protected NavMenu NavMenu { get; set; }
        protected IArticleService articleService { get; set; }
        protected IArticleFrontEndService articlesFrontendAppService { get; set; }

        protected INavigationService navigationService { get; set; }

        private int categoryId;
        private string categoryName;
        private string _SearchText = "";
        private bool isError = false;


        private ItemsProviderResult<ArticleModel> articleDto;
        private readonly GetArticlesByCatInput _filter = new GetArticlesByCatInput();
        private Virtualize<ArticleModel> ArticlesContainer { get; set; }

        public Index()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articlesAppService = DependencyResolver.Resolve<IArticlesAppService>();
            articlesFrontendAppService = DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();

        }
        protected override async Task OnInitializedAsync()
        {
            //var uri = new Uri(NavigationManager.Uri);
            //var query = uri.Query;
            //var parsedQuery = System.Web.HttpUtility.ParseQueryString(query);
            //var categoryIdValue = parsedQuery["categoryId"];
            //var categoryNameText = parsedQuery["categoryName"];
            //if (!string.IsNullOrEmpty(categoryIdValue))
            //{
            //    if (int.TryParse(categoryIdValue, out var parsedCategoryId))
            //    {
            //        categoryId = parsedCategoryId;
            //    }
            //}
            //if (!string.IsNullOrEmpty(categoryNameText))
            //{
            //    categoryName = categoryNameText;
            //}


            await SetPageHeader(L("Đào tạo kỹ năng"), new List<Services.UI.PageHeaderButton>()
            {
                //new Services.UI.PageHeaderButton(L("CreateNewTenant"), OpenCreateModal)
            });
        }
        private async Task RefreshList()
        {
            _SearchText = _filter.Filter;
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadArticles(new ItemsProviderRequest());
            //await LoadPostOther(new ItemsProviderRequest());
            //await LoadTieuDiem(new ItemsProviderRequest());
        }

        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadArticles(ItemsProviderRequest request)
        {
            //_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            //_filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Filter = _SearchText;
            _filter.CategoryId = 4;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articlesFrontendAppService.GetArticlesByCategory(_filter),
            async (result) =>
            {
                //var articlesFilter = result.Items.ToList();
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items);
                foreach (var model in articlesFilter)
                {
                    model.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(model.PrimaryImageUrl));
                }
                articleDto = new ItemsProviderResult<ArticleModel>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
            }
        );

            return articleDto;
        }
        public async Task ViewArticle(ArticleModel article)
        {
            navigationService.NavigateTo($"ArticleDetail?Id={article.Id}");
            //navigationService.NavigateTo($"Detail4?Id={article.Id}&PrimaryImageUrl={article.PrimaryImageUrl}");
        }

    }
}
