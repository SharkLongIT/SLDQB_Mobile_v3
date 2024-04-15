using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Articles;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Core.Threading;
using Abp.Threading;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Pages.InforNTD;
using BBK.SaaS.Mobile.MAUI.Pages.TinTuc;
using BBK.SaaS.Introduce;
using BBK.SaaS.Mdls.Cms.Introduces;

namespace BBK.SaaS.Mobile.MAUI.Pages.VanBanMoi
{
    public partial class Detail7 : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IArticlesAppService articlesAppService { get; set; }
        protected IArticleFrontEndService articleFrontEndService { get; set; }

        protected IIntroduceAppService introduceAppService { get; set; }


        protected IArticleService articleService { get; set; }

        private Virtualize<ArticleModel> ArticlesContainer { get; set; }


        [Parameter]
        public long Id { get; set; } = 0;
        private MarkupString htmlContent1;

        protected ArticleModel Model = new();
        public Detail7()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articleFrontEndService = DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Chi tiết tin tức"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("Detail7") + "Detail7".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);

            if (q1["Id"] != null)
            {
                Id = long.Parse(q1["Id"]);
            }
       
            try
            {
                ArticleViewDto articleViewDto = await articlesAppService.GetFEArticleDetail(new EntityDto<long> { Id = Id });
                Model = new ArticleModel();
                Model.Id = Id;
                Model.Title = articleViewDto.Title;
                Model.CategoryId = 7;
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

                await LoadOtherArticles(new ItemsProviderRequest());
                await UpdateImage();
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

        private ItemsProviderResult<ArticleModel> articleDto1;
        private readonly GetArticlesByCatInput _filterCat = new GetArticlesByCatInput();

        #region bài viết cùng chuyên mục
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadOtherArticles(ItemsProviderRequest request)
        {
            _filterCat.CategoryId = 7;
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articleFrontEndService.GetArticlesByCategory(_filterCat),
            async (result) =>
            {
                // var articlesFilter = result.Items.ToList();
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Where(result => result.Id != Model.Id)).ToList();
                foreach (var article in articlesFilter)
                {

                    article.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(article.PrimaryImageUrl));

                }
                articleDto1 = new ItemsProviderResult<ArticleModel>(articlesFilter, articlesFilter.Count);
                await UserDialogsService.UnBlock();
                StateHasChanged();
            }
        );

            return articleDto1;

        }
        #endregion

        #region Bài viết mới nhất
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadNewArticles(ItemsProviderRequest request)
        {
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articleFrontEndService.GetNewestArticles(),
            async (result) =>
            {
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Where(result => result.Id != Model.Id)).Take(8).ToList();
                foreach (var article in articlesFilter)
                {

                    article.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(article.PrimaryImageUrl));

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
            navigationService.NavigateTo($"Detail7?Id={article.Id}");
            await OnInitializedAsync();
            await ArticlesContainer.RefreshDataAsync();
            StateHasChanged();

        }
        private IntroduceModal introduceModal { get; set; }
        private bool IsUserLoggedIn;

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

    }

}
