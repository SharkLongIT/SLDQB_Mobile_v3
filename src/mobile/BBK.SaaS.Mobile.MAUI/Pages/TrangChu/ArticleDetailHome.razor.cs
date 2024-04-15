using Abp.Application.Services.Dto;
using Abp.Threading;
using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Introduce;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Pages.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.TinTuc;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.TrangChu
{
    public partial class ArticleDetailHome : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IArticlesAppService articlesAppService { get; set; }
        protected IArticleFrontEndService articlefrontendAppService { get; set; }
        protected IIntroduceAppService introduceAppService { get; set; }

        private readonly SearchArticlesInput _filter = new SearchArticlesInput();
        protected IArticleService articleService { get; set; }


        private Virtualize<ArticleModel> ArticlesContainer { get; set; }

        private ItemsProviderResult<ArticleModel> articleDto1;
        private readonly GetArticlesByCatInput _filterCat = new GetArticlesByCatInput();

        [Parameter]
        public long Id { get; set; } = 0;
        public long CategoryId;
        public string PrimaryImageUrl;
        private MarkupString htmlContent1;

        protected ArticleModel Model = new();
        public ArticleDetailHome()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articlesAppService = DependencyResolver.Resolve<IArticlesAppService>();
            articlefrontendAppService= DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Chi tiết tin tức"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ArticleDetailHome") + "ArticleDetailHome".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);

            if (q1["Id"] != null)
            {
                Id = long.Parse(q1["Id"]);
            }
            if (q1["CategoryId"] != null)
            {
                CategoryId = long.Parse(q1["CategoryId"]);
            }
            if (q1["PrimaryImageUrl"] != null)
            {
                PrimaryImageUrl = (q1["PrimaryImageUrl"]);
            }
            try
            {
                ArticleViewDto articleViewDto = await articlefrontendAppService.GetArticleDetail(new EntityDto<long> { Id = Id });
                Model = new ArticleModel();
                Model.Title = articleViewDto.Title;
                Model.Id = Id;
                Model.ShortDesc = articleViewDto.ShortDesc;
                //Model.Content = articleViewDto.Content;
                Model.Slug = articleViewDto.Slug;
                Model.AllowComments = articleViewDto.AllowComments;
                Model.MetaKeywords = articleViewDto.MetaKeywords;
                Model.MetaDescription = articleViewDto.MetaDescription;
                Model.MetaTitle = articleViewDto.MetaTitle;
                Model.OgTitle = articleViewDto.OgTitle;
                Model.OgDescription = articleViewDto.OgDescription;
                Model.OgImageUrl = articleViewDto.OgImageUrl;
                Model.PrimaryImageUrl = articleViewDto.PrimaryImageUrl;
                Model.Author = articleViewDto.Author;
                 Model.LastModificationTime = articleViewDto.Modified;
                Model.ViewedCount = articleViewDto.ViewedCount;
                htmlContent1 = new MarkupString(articleViewDto.Content);

                //var category = articleViewDto.Categories.FirstOrDefault();
                //CategoryId = category.Id;

                await UpdateImage();
                //await LoadOtherArticles(new ItemsProviderRequest());
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }
        private async Task RefeshList()
        {
            await OnInitializedAsync();
            StateHasChanged();

        }
        public Task UpdateImage()
        {
            Model.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.PrimaryImageUrl));
            return Task.CompletedTask;
        }




        #region bài viết cùng chuyên mục
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadOtherArticles(ItemsProviderRequest request)
        {
            _filterCat.CategoryId = CategoryId;
            _filterCat.Filter = "";
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articlefrontendAppService.GetArticlesByCategory(_filterCat),
            async (result) =>
            {
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Where(result => result.Id != Model.Id)).ToList();
                foreach (var item in articlesFilter)
                {
                    item.PrimaryImageUrl = await articleService.GetPicture(item.PrimaryImageUrl);
                }
                articleDto1 = new ItemsProviderResult<ArticleModel>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
            }
        );

            return articleDto1;
        }
        #endregion

        public async Task ViewArticle(ArticleModel article)
        {
            var articleViewDto = await articlefrontendAppService.GetArticleDetail(new EntityDto<long> { Id = article.Id.Value });
            var category = articleViewDto.Categories.FirstOrDefault();
            CategoryId = category.Id;
            navigationService.NavigateTo($"ArticleDetailHome?Id={article.Id}&PrimaryImageUrl={article.PrimaryImageUrl}&CategoryId={CategoryId}");
            await OnInitializedAsync();
            await ArticlesContainer.RefreshDataAsync();
        }
        private IntroduceModal introduceModal { get; set; }
        public async Task Introduce()
        {
            try
            {
                var countCheck = await introduceAppService.GetCountByUserIdForMobile();
                if (countCheck >= 10)
                {
                    await UserDialogsService.AlertError("Bạn đã giới thiệu 10 lần trong ngày hôm nay", "Vui lòng quay lại vào ngày mai");
                }
                else
                {
                    await introduceModal.OpenFor(Model);
                }
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }


        }

    }

}
