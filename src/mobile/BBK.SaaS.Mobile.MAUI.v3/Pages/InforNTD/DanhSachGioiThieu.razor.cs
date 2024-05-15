using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class DanhSachGioiThieu : SaaSMainLayoutPageComponentBase
    {
        protected IIntroduceAppService introduceAppService {  get; set; }
        protected INavigationService navigationService { get; set; }


        private ItemsProviderResult<IntroduceEditDto> introduceDto;
        private readonly IntroduceSearch _filter = new IntroduceSearch();
        private Virtualize<IntroduceEditDto> IntroduceContainer { get; set; }

        private bool isError = false;
        public string _Search = "";
        public int? _Status;
        private bool _IsCancelList;

        public DanhSachGioiThieu() 
        {
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();

        }
        protected override async Task OnInitializedAsync()
        {
        }
        private async Task RefeshList()
        {
            _IsCancelList = true;

            _Search = _filter.Search;
            _Status = _filter.Status;
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
            _filter.Status = _Status;
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await introduceAppService.GetAllByUserType(_filter),
                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<IntroduceEditDto>>(result.Items);
                    foreach (var item in jobFilter)
                    {
                        if (item.Phone.StartsWith("+84"))
                        {
                            item.Phone = item.Phone.Replace("+84", "").Trim();
                        }
                    }
                    if (jobFilter.Count == 0)
                    {
                        isError = true;
                    }
                    else
                    {
                        isError = false;

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
