using Abp.Threading;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
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


        private ItemsProviderResult<CmsCatDto> cmsCatDto;
        private readonly GetCmsCatInput getCmsCatInput = new GetCmsCatInput();
        private Virtualize<CmsCatDto> CmsContainer { get; set; }

        private ItemsProviderResult<ArticleModel> articleDto;
        private readonly GetArticlesByCatInput _filter = new GetArticlesByCatInput();

        private readonly SearchArticlesInput _filterArticle = new SearchArticlesInput();

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
            _SearchText = _filterArticle.Filter;
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadArticles(new ItemsProviderRequest());
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();

        }
        #region Category
        private async ValueTask<ItemsProviderResult<CmsCatDto>> LoadCategories(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await cmsCatsAppService.GetCmsCats(),
            async (result) =>
            {
                var articlesFilter = result.Items.ToList();
                cmsCatDto = new ItemsProviderResult<CmsCatDto>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
            }
        );

            return cmsCatDto;
        }
        #endregion


        #region Article
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadArticles(ItemsProviderRequest request)
        {
            _filterArticle.Filter = _SearchText;
            

            await UserDialogsService.Block();

                await WebRequestExecuter.Execute(
                async () => await articlesFrontendAppService.GetArticles(_filterArticle),
                async (result) =>
                {
                    var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items);
                    if (_SearchText != "")
                    {
                        if (articlesFilter.Count == 0)
                        {
                            isError = true;
                        }
                        else
                        {
                            isError = false;
                        }
                    }
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
        }
        public async Task GetArticleByCategory(CmsCatDto cmsCatDto)
        {
            navigationService.NavigateTo($"ListArticle?CategoryId={cmsCatDto.Id}&CategoryName={cmsCatDto.DisplayName}");
        }
        #endregion
    }
}
