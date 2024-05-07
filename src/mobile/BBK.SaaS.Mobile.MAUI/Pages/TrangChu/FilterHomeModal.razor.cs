using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using Microsoft.AspNetCore.Components;
using Abp.UI;
using static Android.Graphics.ColorSpace;

namespace BBK.SaaS.Mobile.MAUI.Pages.TrangChu
{
    public partial class FilterHomeModal : ModalBase
    {
        protected NavMenu NavMenu { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IArticlesAppService articlesAppService { get; set; }
        protected INavigationService navigationService { get; set; }

        protected IGeoUnitAppService geoUnitAppService { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }
        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private Virtualize<RecruitmentDto> RecruitmentContainer { get; set; }
        private readonly RecruimentInput _filtered = new RecruimentInput();
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        private readonly CatFilterList _filterCat = new CatFilterList();
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        private readonly RecruimentInput _filter = new RecruimentInput();

        public override string ModalId => "filter-home";
        [Parameter] public EventCallback OnSave { get; set; }

        private string _SearchText = "";
        private long _Job;
        private long _WorkSite;
        private decimal _Salary;
        private decimal _SalaryMax;
        private long _Experience;
        private long _Degree;
        private List<GeoUnitDto> _workSite { get; set; }
        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
        }
        private List<CatUnitDto> _career { get; set; }
        public List<CatUnitDto> Career
        {
            get => _career;
            set => _career = value;
        }

        public FilterHomeModal()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();

        }
        bool _isInitialized;
        public async Task OpenFor()
        {
            _isInitialized = false;
            try
            {


                await SetBusyAsync(async () =>
                {

                  
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                            await LoadGeoUnit(new ItemsProviderRequest());
                            await LoadFilter(new ItemsProviderRequest());
                            _isInitialized = true;
                        }
                    );;

                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            await Show();
        }


        #region filter
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {
                 _career = catUnit.Career;


                 var filterList = new CatFilterList()
                 {
                     Career = _career,
                 };

                 CatFilterListDto = new ItemsProviderResult<CatFilterList>(
                     items: new List<CatFilterList> { filterList },
                     totalItemCount: 1
                 );
             });

            await UserDialogsService.UnBlock();
            return CatFilterListDto;


        }

        #endregion
        #region Địa điểm
        private async ValueTask<ItemsProviderResult<GeoUnitDto>> LoadGeoUnit(ItemsProviderRequest request)
        {

            await WebRequestExecuter.Execute(
                async () => await geoUnitAppService.GetGeoUnits(),
                async (result) =>
                {
                    var geoUnit = result.Items.Where(x => x.ParentId == null).ToList();
                    int totalItems = 1;

                    geoUnitDto = new ItemsProviderResult<GeoUnitDto>(geoUnit, totalItems);
                    if (geoUnitDto.Items != null)
                    {
                        WorkSite = geoUnit;
                    }
                });

            await UserDialogsService.UnBlock();

            return geoUnitDto;
        }

        #endregion


        private async Task RefeshList()
        {
            _SearchText = _filtered.Filtered;
            await Hide();
            navigationService.NavigateTo($"ViecTimNguoi?_SearchText={_SearchText}&Job={_Job}&Worksite={_WorkSite}");
        }


        public async void FilterWorksite(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            _WorkSite = select;

        }
        public async void FilterJob(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            _Job = select;

        }
    }
}
