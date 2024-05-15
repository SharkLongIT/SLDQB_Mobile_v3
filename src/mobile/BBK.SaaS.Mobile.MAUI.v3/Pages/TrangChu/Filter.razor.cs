using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.TrangChu
{
    public partial class Filter : SaaSMainLayoutPageComponentBase
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

        public Filter()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();

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
            //if (_filter.WorkSiteId.HasValue)
            //{
            //    _WorkSite = _filter.WorkSiteId.Value;
            //}
            //if (_filter.Job.HasValue)
            //{
            //    _Job = _filter.Job.Value;
            //}

            await UriFilter();
        }

        public async Task UriFilter()
        {
            navigationService.NavigateTo($"ViecTimNguoi?_SearchText={_SearchText}&Job={_Job}&WorkSite={_WorkSite}");
        }
        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitment(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Filtered = _SearchText;
            _filter.Job = _Job;
            _filter.SalaryMax = _SalaryMax;
            _filter.Salary = _Salary;
            _filter.Experience = _Experience;
            _filter.Degree = _Degree;
            _filter.WorkSiteId = _WorkSite;
            await UserDialogsService.Block();

            //var catuint = await CatUnitAppService.GetFilterList();


            await WebRequestExecuter.Execute(
                async () => await recruitmentAppService.GetAllUser(_filter),

                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<RecruitmentDto>>(result.Items.Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Take(10));
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();
                });

            return recruitmentDto;
        }
        public async Task LoadFilterTitle(RecruitmentDto recruitmentDto)
        {
            _filtered.Filtered = recruitmentDto.Title;
            await RefeshList();
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
        bool IsOpenFilter;

        public async Task OpenFilter()
        {
            IsOpenFilter = true;
        }
        public async Task CloseFilter()
        {
            IsOpenFilter = false;
        }

        private FilterHomeModal filterHomeModal { get; set; }
        public async Task OpenFilterHomeModal()
        {
            await filterHomeModal.OpenFor();
        }
    }
}
