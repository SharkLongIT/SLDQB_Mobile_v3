using Abp.Threading;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class DSCVDaUngTuyen : SaaSMainLayoutPageComponentBase
    {
        protected INavigationService navigationService { get; set; }
        protected IApplicationRequestAppService applicationRequestAppService { get; set; }
        private string _SearchText = "";
        private bool isError = false;
        private ItemsProviderResult<ApplicationRequestEditDto> applicationRequestDto;
        private readonly ApplicationRequestSearch _filter = new ApplicationRequestSearch();
        private Virtualize<ApplicationRequestEditDto> ApplicationRequestContainer { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected IArticleService articleService { get; set; }

        protected IRecruitmentAppService recruitmentAppService {  get; set; }    

        private bool IsUserLoggedIn;
        private string _userImage;
        private bool _IsCancelList;

        public DSCVDaUngTuyen()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            applicationRequestAppService = DependencyResolver.Resolve<IApplicationRequestAppService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();
            await GetUserPhoto();
        }
        private async Task RefeshList()
        {
            _IsCancelList = true;
            _SearchText = _filter.Search;

            await ApplicationRequestContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadApplicationRequest(new ItemsProviderRequest());
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            await ApplicationRequestContainer.RefreshDataAsync();
            StateHasChanged();

        }
        private async ValueTask<ItemsProviderResult<ApplicationRequestEditDto>> LoadApplicationRequest(ItemsProviderRequest request)
        {

            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Search = _SearchText;


            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await applicationRequestAppService.GetAll(_filter),
                async (result) =>
                {
                    var recruitmentPost = result.Items.OrderByDescending(x => x.CreationTime).ToList();
                    foreach (var model in recruitmentPost)
                    {
                        model.Recruitment.Recruiter.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(model.Recruitment.Recruiter.AvatarUrl));
                    }
                    if (_SearchText != "")
                    {
                        if (recruitmentPost.Count == 0)
                        {
                            isError = true;
                        }
                        else
                        {
                            isError = false;
                        }
                    }
                    applicationRequestDto = new ItemsProviderResult<ApplicationRequestEditDto>(recruitmentPost, recruitmentPost.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return applicationRequestDto;
        }
        public async Task ViewApplication(ApplicationRequestEditDto applicationRequest)
        {
            navigationService.NavigateTo($"ChiTietCVDUT?Id={applicationRequest.Id}");
        }
        public async Task ViewJob(ApplicationRequestEditDto applicationRequest)
        {
            var recruitment = await recruitmentAppService.GetDetail(applicationRequest.Recruitment.Id.Value);
            navigationService.NavigateTo($"ThongTinVTN?Id={applicationRequest.RecruitmentId}&Experiences={recruitment.Experiences.DisplayName}");

        }
        public async Task ViewCompany(ApplicationRequestEditDto applicationRequest)
        {
            navigationService.NavigateTo($"CompanyDetail?RecruiterId={applicationRequest.Recruitment.Recruiter.UserId}");
        }
        private async Task GetUserPhoto()
        {
            if (!IsUserLoggedIn)
            {
                return;
            }

            _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
            StateHasChanged();
        }
        private async void DisPlayAction(ApplicationRequestEditDto applicationRequest)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Hành động",null,null,"Xem công việc", "Xem trang công ty", "Xem lại CV" );

            if (response == "Xem công việc")
            {
                await ViewJob(applicationRequest);
            }
            else if (response == "Xem trang công ty")
            {
                await ViewCompany(applicationRequest);
            }
            else
            {
               await ViewApplication(applicationRequest);
            }
        }
    }
}

