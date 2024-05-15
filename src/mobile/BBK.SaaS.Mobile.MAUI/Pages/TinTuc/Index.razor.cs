using Abp.Threading;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.TinTuc
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        protected IArticlesAppService articlesAppService { get; set; }
        protected IArticleFrontEndService articlesFrontendAppService { get; set; }
        protected ICmsCatsAppService cmsCatsAppService { get; set; }
        protected NavMenu NavMenu { get; set; }
        protected IArticleService articleService { get; set; }
        protected INavigationService navigationService { get; set; }
        private string _SearchText = ""; //search theo vi tri
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
            cmsCatsAppService = DependencyResolver.Resolve<ICmsCatsAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Tin tức"));
        }
        private async Task RefeshList()
        {
            _SearchText = _filter.Filter;
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadArticles(new ItemsProviderRequest());
        }

        //#region Category
        //private async ValueTask<ItemsProviderResult<CmsCatDto>> LoadCategories(ItemsProviderRequest request)
        //{
        //    _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
        //    _filter.SkipCount = request.StartIndex;
        //    //_filter.Take = Math.Clamp(request.Count, 1, 1000);

        //    await UserDialogsService.Block();

        //    await WebRequestExecuter.Execute(
        //    async () => await cmsCatsAppService.GetCmsCatsByLevel(_filterCmsCat),
        //    async (result) =>
        //    {
        //        var articlesFilter = result.Items.ToList();
        //        cmsCatDto = new ItemsProviderResult<CmsCatDto>(articlesFilter, articlesFilter.Count);
        //        await UserDialogsService.UnBlock();
        //    }
        //);

        //    return cmsCatDto;
        //}
        //private void NavigateToCategory(int categoryId,string categoryName ,MouseEventArgs e)
        //{
        //    navigationService.NavigateTo($"Category?categoryId={categoryId}&categoryName={categoryName}");
        //}
        //#endregion


        #region Article
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadArticles(ItemsProviderRequest request)
        {
            _filter.Filter = _SearchText;
            _filter.CategoryId = 1;

            await UserDialogsService.Block();

                await WebRequestExecuter.Execute(
                async () => await articlesFrontendAppService.GetArticlesByCategory(_filter),
                async (result) =>
                {
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
            navigationService.NavigateTo($"ArticleDetail?Id={article.Id}&CategoryId={1}");
        }

        #endregion
    }
}
