using Abp.Threading;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        protected NavMenu NavMenu { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        private string _SearchText = "";
        private decimal? _SalaryMin;
        private decimal? _SalaryMax;
        private long _Experience;
        private long _Degree;
        private long _Job;
        private long _WorkSite;
        private bool isError = false;



        private string Filter = "";
        private long Job;
        private long Worksite;
        private long ExperienceId;
        private decimal? SalaryMin;
        private decimal? SalaryMax;

        private int RecruitmentCount;
        bool IsRecruitmentCount;
        protected IGeoUnitAppService geoUnitAppService { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }
        protected IArticleService articleService { get; set; }

        public GeoUnitDto geoUnitDtos { get; set; }


        #region CatUnit
        private List<CatUnitDto> _degree { get; set; }
        private List<CatUnitDto> _career { get; set; }
        private List<CatUnitDto> _rank { get; set; }
        private List<CatUnitDto> _formOfWork { get; set; }
        private List<CatUnitDto> _experience { get; set; }
        private List<CatUnitDto> _salary { get; set; }
        private List<CatUnitDto> _staffSize { get; set; }
        private List<GeoUnitDto> _workSite { get; set; }

        public List<CatUnitDto> Degree
        {
            get => _degree;
            set => _degree = value;
        }

        public List<CatUnitDto> Career
        {
            get => _career;
            set => _career = value;
        }

        public List<CatUnitDto> Rank
        {
            get => _rank;
            set => _rank = value;
        }

        public List<CatUnitDto> FormOfWork
        {
            get => _formOfWork; set => _formOfWork = value;
        }

        public List<CatUnitDto> Experience
        {
            get => _experience;
            set => _experience = value;
        }

        public List<CatUnitDto> Salary
        {
            get => _salary;
            set => _salary = value;
        }

        public List<CatUnitDto> StaffSize
        {
            get => _staffSize;
            set => _staffSize = value;
        }
        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
        }
        #endregion

        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;


        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }


        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private Virtualize<RecruitmentDto> RecruitmentContainer { get; set; }
        private  RecruimentInput _filter = new RecruimentInput();
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        protected IApplicationContext ApplicationContext { get; set; }

        public Index()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        protected override async Task OnInitializedAsync()
        {

            await SetPageHeader(L("Danh sách Việc tìm người"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ViecTimNguoi") + "ViecTimNguoi".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            if (q1["_SearchText"] != null)
            {
                Filter = (q1["_SearchText"]);
            }
            if (q1["Job"] != null)
            {
                Job = int.Parse(q1["Job"]);
            }
            if (q1["WorkSite"] != null)
            {
                Worksite = int.Parse(q1["WorkSite"]);
            }
            if (q1["Experience"] != null)
            {
                ExperienceId = int.Parse(q1["Experience"]);
            }
            if (q1["SalaryMin"] != null && q1["SalaryMin"] != "")
            {
                SalaryMin = decimal.Parse(q1["SalaryMin"]);
            }
            else
            {
                SalaryMin = null;
            }
            if (q1["SalaryMax"] != null && q1["SalaryMax"] != "")
            {
                SalaryMax = decimal.Parse(q1["SalaryMax"]);
            }
            else
            {
                SalaryMax =null;
            }
            await ReloadList();
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            //IsFilter = true;
            //if (_SearchText == "")
            //{
            //    IsFilter = false;
            //}
            await RecruitmentContainer.RefreshDataAsync();
            StateHasChanged();
            //await LoadRecruitment(new ItemsProviderRequest());

        }
        private async Task ReloadList()
        {
            #region InPage
          
            if (Filter == null)
            {
                _SearchText = _filter.Filtered;

            }
            else
            {
                _SearchText = Filter;
            };
            _WorkSite = Worksite;
            _Job = Job;
            _Experience = ExperienceId;
            if (SalaryMin.HasValue)
            {

                _SalaryMin = SalaryMin.Value;
                IsCancel = true;
            }
            if (SalaryMax.HasValue)
            {
                _SalaryMax = SalaryMax.Value;
                IsCancel = true;
            }
            _Degree = 0;
            #endregion

            await RecruitmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadRecruitment(new ItemsProviderRequest());
        }
        private bool IsCancel;
        //private async Task RefeshList()
        //{
        //    IsCancel = true;
        //    IsRecruitmentCount = true;
        //    IsOpenFilter = false;
        //    #region InPage
        //    _SearchText = _filter.Filtered;
        //    if (_filter.Salary.HasValue)
        //    {

        //    _SalaryMin = _filter.Salary.Value;
        //    } 
        //    if (_filter.SalaryMax.HasValue)
        //    {

        //    _SalaryMax = _filter.SalaryMax.Value;
        //    }   
        //    _Experience = _filter.Experience.Value;
        //    _Degree = _filter.Degree.Value;
        //    _Job = _filter.Job.Value;
        //    _WorkSite = _filter.WorkSiteId.Value;
        //    #endregion

        //    await RecruitmentContainer.RefreshDataAsync();
        //    StateHasChanged();
        //    await LoadRecruitment(new ItemsProviderRequest());
        //}
        private async Task RefeshListAfterFillter()
        {
            IsCancel = true;
            IsRecruitmentCount = true;
            IsOpenFilter = false;
            #region InPage
            _filter = fillterModal._filter;
            _SearchText = _filter.Filtered;
            if (_filter.Salary.HasValue)
            {

                _SalaryMin = _filter.Salary.Value;
            }
            if (_filter.SalaryMax.HasValue)
            {

                _SalaryMax = _filter.SalaryMax.Value;
            }
            if (_filter.Experience.HasValue) { _Experience = _filter.Experience.Value; }
            if (_filter.Degree.HasValue) { _Degree = _filter.Degree.Value; }

            if (_filter.Job.HasValue) { _Job = _filter.Job.Value; }
            
            if (_filter.WorkSiteId.HasValue) { _WorkSite = _filter.WorkSiteId.Value; }
           
            #endregion

            await RecruitmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadRecruitment(new ItemsProviderRequest());
        }
        private async Task CancelList()
        {
            _SearchText = "";
            _SalaryMin = null;
            _SalaryMax = null;
            _Experience = 0;
            _Degree = 0;
            _Job = 0;
            _WorkSite = 0;
            IsCancel = false;
            await RecruitmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadRecruitment(new ItemsProviderRequest());

        }

        #region

        public CatFilterList catFilterList { get; set; }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),
           
             async (catUnit) =>
             {
                 _degree = catUnit.Degree;
                 _career = catUnit.Career;
                 _experience = catUnit.Experience;
                 _formOfWork = catUnit.FormOfWork;
                 _rank = catUnit.Rank;

                 var filterList = new CatFilterList()
                 {
                     Degree = _degree,
                     Career = _career,
                     Experience = _experience,
                     FormOfWork = _formOfWork,
                     Rank = _rank
                    
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


        #region GeoUnit

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
        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitment(ItemsProviderRequest request)
        {
            //_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            //_filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Filtered = _SearchText;
            _filter.Job = _Job;
            _filter.SalaryMax = _SalaryMax;
            _filter.Salary = _SalaryMin;
            _filter.Experience = _Experience;
            _filter.Degree = _Degree;
            _filter.WorkSiteId = _WorkSite;

            await UserDialogsService.Block();

            //var catuint = await CatUnitAppService.GetFilterList();


            await WebRequestExecuter.Execute(
                async () => await recruitmentAppService.GetAllUser(_filter),

                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<RecruitmentDto>>(result.Items.Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0));
                    foreach (var model in jobFilter)
                    {
                        model.Recruiter.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(model.Recruiter.AvatarUrl));
                    }
                    RecruitmentCount = jobFilter.Count;
                    if (_SearchText != "")
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
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(jobFilter, jobFilter.Count);


                    await UserDialogsService.UnBlock();

                }
            ); ;

            return recruitmentDto;
        }

        public async Task ViewRecruitment(RecruitmentDto recruitment)
        {
            navigationService.NavigateTo($"ThongTinVTN?Id={recruitment.Id}&WorkAddress={recruitment.WorkAddress}&HumanResSizeCat={recruitment.Recruiter.HumanResSizeCat.DisplayName}&Experiences={recruitment.Experiences.DisplayName}");
        }

        bool IsOpenFilter;
        FillterModal fillterModal = new FillterModal() ;
        public async Task OpenFilter()
        {
            IsOpenFilter = true;
            await fillterModal.OpenFor();
        }
        public async Task CloseFilter()
        {
            IsOpenFilter = false;
        }





    }
}
