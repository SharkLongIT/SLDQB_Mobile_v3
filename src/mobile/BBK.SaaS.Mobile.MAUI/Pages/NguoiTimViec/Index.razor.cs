using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        protected NavMenu NavMenu { get; set; }

        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected ICatUnitAppService CatUnitAppService;

        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        private readonly CatFilterList _filterCat = new CatFilterList();

        private bool IsDefault1;
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        protected IGeoUnitAppService geoUnitAppService { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }


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

        protected INavigationService navigationService { get; set; }
        private bool isError = false;
        private int? _Gender;
        private long? _LiteracyId;
        private long? _ExperiencesId;
        private long _WorkSite;
        private long? _OccupationId;
        private decimal? _SalaryMin;
        private decimal? _SalaryMax;
        private string _avatarCandidate;

        private long OccupationId;
        private long Worksite;
        private long ExperienceId;
        private long DegreeId;
        private decimal? SalaryMin;
        private decimal? SalaryMax;


        private int JobAppCount;
        bool IsJobAppCount;
        [Inject]
        protected NavigationManager NavigationManager { get; set; }


        protected IApplicationContext ApplicationContext { get; set; }
        private bool IsUserLoggedIn;
        private bool IsCancel;
        private ItemsProviderResult<NTDDatLichModel> jobApplicationDto;
        private readonly JobAppSearch _filter = new JobAppSearch();
        public Index()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        protected override async Task OnInitializedAsync()
        {
           
            //await SetPageHeader(L("Danh sách Người tìm việc"));
        }
        private async Task RefeshList()
        {
            IsOpenFilter = false;
            IsJobAppCount = true;
            _Gender = _filter.Gender;
            _LiteracyId = _filter.LiteracyId;
            _ExperiencesId = _filter.ExperiencesId; // kinh nghiem
            _WorkSite = _filter.WorkSiteId.Value; // dia diem
            _OccupationId = _filter.OccupationId; // nghe nghiep
            if (_filter.SalaryMin.HasValue) {

                _SalaryMin = _filter.SalaryMin.Value;
            }
            if (_filter.SalaryMax.HasValue)
            {
                _SalaryMax = _filter.SalaryMax.Value;
            } 
            await JobApplicationContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadJobApplication(new ItemsProviderRequest());
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




        private Virtualize<NTDDatLichModel> JobApplicationContainer { get; set; }
        private async ValueTask<ItemsProviderResult<NTDDatLichModel>> LoadJobApplication(ItemsProviderRequest request)
        {
            //_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            //_filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.LiteracyId = _LiteracyId;
            _filter.Gender = _Gender;
            _filter.ExperiencesId = _ExperiencesId;
            _filter.WorkSiteId = _WorkSite;
            _filter.OccupationId = _OccupationId;
            _filter.SalaryMin = _SalaryMin;
            _filter.SalaryMax = _SalaryMax;
            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await jobApplicationAppService.GetAllJobAppsMobile(_filter),
                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<NTDDatLichModel>>(result.Items);
                    //foreach (var item in jobFilter)
                    //{
                    //    _avatarCandidate = await UserProfileService.GetProfilePicture(item.Candidate.UserId);
                    //    item.Candidate.AvatarUrl = _avatarCandidate;
                    //}
                    JobAppCount = jobFilter.Count;  
                    if (jobFilter.Count == 0)
                    {
                        isError = true;
                    }
                    else
                    {
                        isError = false;

                    }
                    jobApplicationDto = new ItemsProviderResult<NTDDatLichModel>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();

                }
            );

            return jobApplicationDto;
        }

        public async Task ViewUser(NTDDatLichModel jobApplication)
        {
            navigationService.NavigateTo($"ThongTinNTV?Id={jobApplication.JobApplication.Id}&Positions={jobApplication.JobApplication.Positions.DisplayName}&FormOfWork={jobApplication.JobApplication.FormOfWork.DisplayName}&Literacy={jobApplication.JobApplication.Literacy.DisplayName}");
        }



        //đặt lịch phỏng vấn
        private DatLichPVModal datLichPVModal { get; set; }
        //public async Task BookUser(NTDDatLichModel nTDDatLichModel)
        //{
        //    if (ApplicationContext.LoginInfo == null)
        //    {
        //        await UserDialogsService.AlertWarn("Vui lòng đăng nhập để đặt lịch!");
        //        //navigationService.NavigateTo(NavigationUrlConsts.Login);
        //    }
        //    else
        //    {
        //        await datLichPVModal.OpenFor(nTDDatLichModel);
        //    }
        //}
        public static string GetTimeSince(DateTime objDateTime)
        {
            string result = string.Empty;
            var timeSpan = DateTime.Now.Subtract(objDateTime);

            if (timeSpan <= TimeSpan.FromSeconds(60))
            {
                result = string.Format("{0} giây trước", Math.Round(timeSpan.TotalSeconds));
            }
            else if (timeSpan <= TimeSpan.FromMinutes(60))
            {
                result = timeSpan.Minutes > 1 ?
                    String.Format("{0} phút trước", Math.Round(timeSpan.TotalMinutes)) :
                    "1 phút trước";
            }
            else if (timeSpan <= TimeSpan.FromHours(24))
            {
                result = timeSpan.Hours > 1 ?
                    String.Format("{0} giờ trước", Math.Round(timeSpan.TotalHours)) :
                    "1 giờ trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(30))
            {
                result = timeSpan.Days > 1 ?
                    String.Format("{0} ngày trước", Math.Round(timeSpan.TotalDays)) :
                    "1 ngày trước";
            }
            else if (timeSpan <= TimeSpan.FromDays(365))
            {
                result = timeSpan.Days > 30 ?
                    String.Format("{0} tháng trước", Math.Round(Math.Round(timeSpan.TotalDays) / 30), MidpointRounding.AwayFromZero) :
                    "1 tháng trước";
            }
            else
            {
                result = timeSpan.Days > 365 ?
                    String.Format("{0} năm trước", Math.Round(Math.Round(timeSpan.TotalDays) / 365), MidpointRounding.AwayFromZero) :
                    "1 năm trước";
            }

            return result;
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


    }
}
