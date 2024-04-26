using Abp.Application.Services.Dto;
using Abp.Threading;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Drawing.Design;

namespace BBK.SaaS.Mobile.MAUI.Pages.TinTuc
{
    public partial class ArticleDetail : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IArticleFrontEndService articleFrontendAppService { get; set; }

        protected IIntroduceAppService introduceAppService { get; set; }

        private ItemsProviderResult<ArticleModel> articleDto;
        private readonly SearchArticlesInput _filter = new SearchArticlesInput();

        protected IApplicationContext ApplicationContext { get; set; }
        protected IFECntCategoryAppService fECntCategoryAppService { get; set; }
        private Virtualize<ArticleModel> ArticlesContainer { get; set; }

        private ItemsProviderResult<ArticleModel> articleDto1;
        private readonly GetArticlesByCatInput _filterCat = new GetArticlesByCatInput();
        protected IArticleService articleService { get; set; }

        [Parameter]
        public long Id { get; set; } = 0;
        public long CategoryId { get; set; } = 0;
        public string CategoryName;
        public string PrimaryImageUrl;
        private bool _IsInitLoadOther;
        private bool _IsInitLoadNew;
        private bool _IsInitialication;

        private MarkupString htmlContent1;

        protected ArticleModel Model = new();
        public ArticleDetail()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articleFrontendAppService = DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();
            fECntCategoryAppService = DependencyResolver.Resolve<IFECntCategoryAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }

        protected override async Task OnInitializedAsync()
        {
            _IsInitialication = false; 
            await SetPageHeader(L("Chi tiết tin tức"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ArticleDetail") + "ArticleDetail".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);

            if (q1["Id"] != null)
            {
                Id = long.Parse(q1["Id"]);
            }
            try
            {
                ArticleViewDto articleViewDto = await articleFrontendAppService.GetArticleDetail(new EntityDto<long> { Id = Id });
                Model = new ArticleModel();
                Model.Title = articleViewDto.Title;
                Model.Id = Id;
               // Model.CategoryId = 1;
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
                var categoryID = articleViewDto.Categories.FirstOrDefault();
                CategoryId = categoryID.Id;
                Model.CategoryId = categoryID.Id;
                await LoadOtherArticles(new ItemsProviderRequest());
                await UpdateImage();
                StateHasChanged();
                _IsInitialication = true;
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
            _IsInitLoadOther = false;
            _filterCat.CategoryId = CategoryId;
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articleFrontendAppService.GetArticlesByCategory(_filterCat),
            async (result) =>
            {
                // var articlesFilter = result.Items.ToList();
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Where(result => result.Id != Model.Id)).Take(5).ToList();
                foreach (var article in articlesFilter)
                {
                    article.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(article.PrimaryImageUrl));
                    //ArticleViewDto articleViewDto = await articleFrontendAppService.GetArticleDetail(new EntityDto<long> { Id = article.Id.Value });
                    //article.Modified = articleViewDto.Modified;
                }
                articleDto1 = new ItemsProviderResult<ArticleModel>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
                _IsInitLoadOther = true;
                StateHasChanged();

            }
        );

            return articleDto1;

        }
        #endregion

        #region Bài viết mới nhất
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadNewArticles(ItemsProviderRequest request)
        {
            _IsInitLoadNew = false;
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articleFrontendAppService.GetNewestArticles(),
            async (result) =>
            {
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Where(result => result.Id != Model.Id)).Take(8).ToList();
                foreach (var article in articlesFilter)
                {

                    article.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(article.PrimaryImageUrl));

                }
                articleDto = new ItemsProviderResult<ArticleModel>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
                _IsInitLoadNew = true;
            }
        );

            return articleDto;
        }
        #endregion
        public async Task ViewArticle(ArticleModel article)
        {
            navigationService.NavigateTo($"ArticleDetail?Id={article.Id}");
            await OnInitializedAsync();
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();

        }

        #region  giới thiệu
        private bool IsUserLoggedIn;

        private IntroduceModal introduceModal { get; set; }
        public async Task Introduce()
        {
            try
            {
                IsUserLoggedIn = navigationService.IsUserLoggedIn();

                if (IsUserLoggedIn == true)
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
        #endregion


        public async Task GetArticleByCategory()
        {
            //var category = await fECntCategoryAppService.GetCategory(new GetCategoryInput()
            //{
            //    CategoryId = Model.CategoryId,
            //    SearchArticlesInput = new SearchArticlesInput() { 

            //        MaxResultCount = 25 ,
            //        CategoryId = Model.CategoryId,
            //        Filter = "",
            //        TenantId = ApplicationContext.CurrentTenant.TenantId,
            //    }
            //});
            //if (category != null)
            //{
            //    CategoryName = category.DisplayName;
            //}
            navigationService.NavigateTo($"ListArticle?CategoryId={Model.CategoryId}");
        }


    }
}
