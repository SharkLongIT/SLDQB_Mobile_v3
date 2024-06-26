﻿using BBK.SaaS.Introduce;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Cms.Introduces;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mobile.MAUI.Pages.InforNTD;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Services.Navigation;
using System.Text.RegularExpressions;
using Abp.Application.Services.Dto;
using BBK.SaaS.TinTuc;
using BBK.SaaS.Mdls.Cms.Articles;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class DSGT : SaaSMainLayoutPageComponentBase
    {
        protected IIntroduceAppService introduceAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IArticleFrontEndService articleFrontEndService { get; set; }


        private ItemsProviderResult<IntroduceEditDto> introduceDto;
        private readonly IntroduceSearch _filter = new IntroduceSearch();
        private Virtualize<IntroduceEditDto> IntroduceContainer { get; set; }

        private bool isError = false;
        public string _Search;
        public DSGT()
        {
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
            articleFrontEndService = DependencyResolver.Resolve<IArticleFrontEndService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Danh sách tin giới thiệu của tôi"));
        }
        private bool _IsCancelList;
        private async Task RefeshList()
        {
            _IsCancelList = true;
            _Search = _filter.Search;

            await IntroduceContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadIntroduce(new ItemsProviderRequest());
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _Search = select;
            await IntroduceContainer.RefreshDataAsync();
            StateHasChanged();

        }
        private async ValueTask<ItemsProviderResult<IntroduceEditDto>> LoadIntroduce(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Search = _Search;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await introduceAppService.GetAllByUserType(_filter),
                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<IntroduceEditDto>>(result.Items.OrderByDescending(x =>x.CreationTime));
                    if (_Search != "")
                    {
                        if (jobFilter.Count == 0)
                        {
                            isError = true;
                        }
                        else
                        {
                            isError = false;

                        }
                    }
                    introduceDto = new ItemsProviderResult<IntroduceEditDto>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return introduceDto;
        }
        private IntroduceDetailModal introduceDetailModal { get; set; }
        public async Task ViewIntroduce(IntroduceEditDto introduceEditDto)
        {
            await introduceDetailModal.OpenFor(introduceEditDto);
        }
        private long CategoryId;

        public async Task ViewArticle(IntroduceEditDto introduceEditDto)
        {
            navigationService.NavigateTo($"ArticleDetail?Id={introduceEditDto.Article.Id}");
        }

        private async void DisPlayAction(IntroduceEditDto introduceEditDto)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Chi tiết tin", "Tin liên kết");
            if (response == "Chi tiết tin")
            {
                await ViewIntroduce(introduceEditDto);
            }
            else if (response == "Tin liên kết")
            {
                await ViewArticle(introduceEditDto);
            }
        }
    }
}
