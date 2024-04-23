using Abp.Application.Services.Dto;
using Abp.Threading;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Authorization.Accounts.Dto;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Entities;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.TrangChu
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        protected NavMenu NavMenu { get; set; }
        private bool IsUserLoggedIn;
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IArticlesAppService articlesAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected IProfileAppService ProfileAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IArticleFrontEndService articleFrontEndService { get; set; }
        protected IArticleService articleService { get; set; }
        private IAccountAppService _accountAppService;

        private string _SearchText = "";
        private bool isError1 = false;
        private bool isError = false;
        private decimal? _DesiredSalary;
        private decimal _Salary;
        private decimal _SalaryMax;
        private long _Experience;
        private long _Degree;
        private long _Job;
        private long _WorkSite;
        private long CategoryId;
        private long _Id;

        private ItemsProviderResult<GetJobApplicationForEditOutput> jobApplicationDto;
        private readonly JobAppSearch _filter = new JobAppSearch();

        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private Virtualize<RecruitmentDto> RecruitmentContainer { get; set; }
        private readonly RecruimentInput _filtered = new RecruimentInput();



        private ItemsProviderResult<ArticleModel> articleDto;
        private readonly SearchArticlesInput _filterArticle = new SearchArticlesInput();
        private Virtualize<ArticleModel> ArticlesContainer { get; set; }

        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        private readonly CatFilterList _filterCat = new CatFilterList();
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }



        protected IGeoUnitAppService geoUnitAppService { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }

        private List<CatUnitDto> _career { get; set; }
        public List<CatUnitDto> Career
        {
            get => _career;
            set => _career = value;
        }
        private List<GeoUnitDto> _workSite { get; set; }
        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
        }
        private string _userImage;
        private string _UserName;
        private Guid? _pictureId;
        private long? _userId;
        private string _avatarCandidate;
        public Index()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            _accountAppService = DependencyResolver.Resolve<IAccountAppService>();
            articlesAppService = DependencyResolver.Resolve<IArticlesAppService>();
            articleFrontEndService = DependencyResolver.Resolve<IArticleFrontEndService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            ProfileAppService = DependencyResolver.Resolve<IProfileAppService>();



        }

        private async Task IsTenantAvailableExecuted(IsTenantAvailableOutput result, string tenancyName)
        {
            var tenantAvailableResult = result;

            switch (tenantAvailableResult.State)
            {
                case TenantAvailabilityState.Available:
                    ApplicationContext.SetAsTenant(tenancyName, tenantAvailableResult.TenantId.Value);
                    ApiUrlConfig.ChangeBaseUrl(tenantAvailableResult.ServerRootAddress);
                    break;
                case TenantAvailabilityState.InActive:
                    await UserDialogsService.UnBlock();
                    await UserDialogsService.AlertError(L("TenantIsNotActive", tenancyName));
                    break;
                case TenantAvailabilityState.NotFound:
                    await UserDialogsService.UnBlock();
                    await UserDialogsService.AlertError(L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override async Task OnInitializedAsync()
        {
            // automatically setup tenant Default
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                    async () => await _accountAppService.IsTenantAvailable(
                        new IsTenantAvailableInput { TenancyName = "Default" }),
                    result => IsTenantAvailableExecuted(result, "Default")
                );
            });
            await SetPageHeader(L("Trang chủ"), new List<Services.UI.PageHeaderButton>()
            {
                //new Services.UI.PageHeaderButton(L("Đăng nhập"), NavLogin)
            });

            //check loggedIn
            IsUserLoggedIn = navigationService.IsUserLoggedIn();
            await GetUserPhoto();
        }

        bool ListUserNull;
        bool IsFilter;
        //public async void selectedValue(ChangeEventArgs args)
        //{
        //    string select = Convert.ToString(args.Value);
        //    _SearchText = select;
        //    IsFilter = true;
        //    if (_SearchText == "")
        //    {
        //        IsFilter = false;
        //    }
        //    await RecruitmentContainer.RefreshDataAsync();
        //    StateHasChanged();
        //    //await LoadRecruitment(new ItemsProviderRequest());

        //}
        private async Task RefeshList()
        {
            //IsFilter = true;
            _SearchText = _filtered.Filtered;
            //_Job = _filtered.Job.Value;
            //_WorkSite = _filtered.WorkSiteId.Value;
            //await RecruitmentContainer.RefreshDataAsync();
            //StateHasChanged();
            //await LoadRecruitment(new ItemsProviderRequest());

            await UriFilter();
        }

        public async Task UriFilter()
        {
            //navigationService.NavigateTo($"ViecTimNguoi?_SearchText={_filtered.Filtered}&Job={_filtered.Job}&WorkSite={_filtered.WorkSiteId}");
            navigationService.NavigateTo($"ViecTimNguoi?_SearchText={_filtered.Filtered}");
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


        #region Tin tức
        private async ValueTask<ItemsProviderResult<ArticleModel>> LoadArticles(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
            async () => await articleFrontEndService.GetArticles(_filterArticle),
            async (result) =>
            {
                var articlesFilter = ObjectMapper.Map<List<ArticleModel>>(result.Items.Take(8));
                foreach (var model in articlesFilter)
                {
                    _Id = model.Id.Value;
                    model.PrimaryImageUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(model.PrimaryImageUrl));
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
        #endregion

        #region Việc tìm người
       public int RecruitmentCount;
        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitment(ItemsProviderRequest request)
        {
            _filtered.MaxResultCount = 6;
            _filtered.SkipCount = request.StartIndex;
            _filtered.Take = 6;
            _filtered.Filtered = _SearchText;
            _filtered.Job = _Job;
            _filtered.SalaryMax = _SalaryMax;
            _filtered.Salary = _Salary;
            _filtered.Experience = _Experience;
            _filtered.Degree = _Degree;
            _filtered.WorkSiteId = _WorkSite;

            await UserDialogsService.Block();
            await WebRequestExecuter.Execute(
                async () => await recruitmentAppService.GetAllUser(_filtered),
                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<RecruitmentDto>>(result.Items.Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Take(6));
                    var recruitmentCount = ObjectMapper.Map<List<RecruitmentDto>>(result.Items.Where(x => x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0).Where(x => x.Status == false && x.DeadlineSubmission.CompareTo(DateTime.Today) >= 0));
                    RecruitmentCount = recruitmentCount.Count;
					foreach (var model in jobFilter)
                    {
                        model.Recruiter.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(model.Recruiter.AvatarUrl));
                    }

                    if (jobFilter.Count == 0)
                    {
                        isError1 = true;
                        IsFilter = false;
                    }
                    else
                    {
                        isError1 = false;
                    }
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return recruitmentDto;

        }
        public async Task ViewRecruitment(RecruitmentDto recruitment)
        {
            navigationService.NavigateTo($"ThongTinVTN?Id={recruitment.Id}&WorkAddress={recruitment.WorkAddress}&HumanResSizeCat={recruitment.Recruiter.HumanResSizeCat.DisplayName}&Experiences={recruitment.Experiences.DisplayName}&SphereOfActivity={recruitment.Recruiter.SphereOfActivity.DisplayName}");
        }
        #endregion

        #region Người tìm việc
       private int JobAppCount;
        private Virtualize<GetJobApplicationForEditOutput> JobApplicationContainer { get; set; }
        public List<GetJobApplicationForEditOutput> getJobApplicationForEditOutputs { get; set; }
        private async ValueTask<ItemsProviderResult<GetJobApplicationForEditOutput>> LoadJobApplication(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = "";

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await jobApplicationAppService.GetAllJobAppsMobile(_filter),

                async (result) =>
                {
                    var jobFilter = result.Items.Take(5).ToList();
                    var jobCount = result.Items.Count;
					JobAppCount = jobCount;

                    //foreach (var item in jobFilter)
                    //{
                    //    _avatarCandidate = await UserProfileService.GetProfilePicture(item.Candidate.UserId);
                    //    item.Candidate.AvatarUrl = _avatarCandidate;
                    //}
                    if (jobFilter.Count == 0)
                    {
                        isError = true;
                    }
                    else
                    {
                        isError = false;
                    }
                    jobApplicationDto = new ItemsProviderResult<GetJobApplicationForEditOutput>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return jobApplicationDto;
        }

        public async Task ViewUser(GetJobApplicationForEditOutput jobApplication)
        {
            navigationService.NavigateTo($"ThongTinNTV?Id={jobApplication.JobApplication.Id}&Positions={jobApplication.JobApplication.Positions.DisplayName}&FormOfWork={jobApplication.JobApplication.FormOfWork.DisplayName}&Literacy={jobApplication.JobApplication.Literacy.DisplayName}");
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

        public async Task NavLogin()
        {
            navigationService.NavigateTo(NavigationUrlConsts.Login);
        }
        private async Task GetUserPhoto()
        {
            if (!IsUserLoggedIn)
            {
                return;
            }

            _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
            _UserName = ApplicationContext.LoginInfo.User.Name;
            StateHasChanged();
        }


        public async Task UriVTN()
        {
            navigationService.NavigateTo(NavigationUrlConsts.ViecTimNguoi);
        } 
        public async Task UriNTV()
        {
            navigationService.NavigateTo(NavigationUrlConsts.NguoiTimViec);
        }
          public async Task UriArticle()
        {
            navigationService.NavigateTo(NavigationUrlConsts.TinTuc);
        }

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

        private bool IsShowForm { get; set; } = false;
        private async Task ShowLogin()
        {
            //ShowClassNameNLD = "menu-active";
            if (IsUserLoggedIn == true)
            {
                IsShowForm = false;
                StateHasChanged();
            }
            else
            {
                IsShowForm = true;
                StateHasChanged();
            }


        }
        private async Task HideLogin()
        {
            //var dom = DependencyResolver.Resolve<DomManipulatorService>();
            //await dom.ClearAllAttributes(JS, "#menu-login-1");
            ////await dom.SetAttribute(JS, "#menu-login-1", "class", "menu-active");
        }
    }
    
}
