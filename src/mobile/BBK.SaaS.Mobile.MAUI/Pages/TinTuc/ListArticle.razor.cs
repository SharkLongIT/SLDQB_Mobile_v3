using Abp.Application.Services.Dto;
using Abp.Threading;
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

namespace BBK.SaaS.Mobile.MAUI.Pages.TinTuc
{
    public partial class ListArticle : SaaSMainLayoutPageComponentBase
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

        public ListArticle()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articlesAppService = DependencyResolver.Resolve<IArticlesAppService>();
            articlesFrontendAppService = DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();

        }
        protected override async Task OnInitializedAsync()
        {
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ListArticle") + "ListArticle".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);

            if (q1["CategoryId"] != null)
            {
                categoryId = int.Parse(q1["CategoryId"]);
            }
            if (q1["CategoryName"] != null)
            {
                categoryName = (q1["CategoryName"]);
            }
        }
        private async Task RefreshList()
        {
            _SearchText = _filter.Filter;
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadArticlesByCategory(new ItemsProviderRequest());
        }

        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadArticlesByCategory(ItemsProviderRequest request)
        {
            _filter.Filter = _SearchText;
            _filter.CategoryId = categoryId;

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
                    var modifed = await articlesFrontendAppService.GetArticleDetail(new EntityDto<long> { Id = model.Id.Value });
                    model.LastModificationTime = modifed.Modified;
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

    }
}
