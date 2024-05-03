using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class BaiTuyenDung : SaaSMainLayoutPageComponentBase
    {
        protected INavigationService navigationService { get; set; }
        protected IArticleService articleService { get; set; }
        private string _SearchText = "";
        private bool isError = false;
        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private readonly RecruimentInput _filter = new RecruimentInput();
        protected IRecruitmentAppService iRecruitmentAppService { get; set; }
        private Virtualize<RecruitmentDto> RecruitmentContrainer { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        private bool IsUserLoggedIn;
        private string _userImage;

        public BaiTuyenDung()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            iRecruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            articleService = DependencyResolver.Resolve<IArticleService>();

        }
        protected override async Task OnInitializedAsync()
        {

            await SetPageHeader(L("Danh sách tin tuyển dụng của tôi"), new List<Services.UI.PageHeaderButton>()
            {
                //new Services.UI.PageHeaderButton(L("Thêm mới tin"), OpenCreateModal)
            });


            IsUserLoggedIn = navigationService.IsUserLoggedIn();

        }
        private async Task RefeshList()
        {
            _SearchText = _filter.Filtered;
            await RecruitmentContrainer.RefreshDataAsync();
            StateHasChanged();
            await LoadRecruitmentPost(new ItemsProviderRequest());
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            await RecruitmentContrainer.RefreshDataAsync();
            StateHasChanged();

        }
        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitmentPost(ItemsProviderRequest request)
        {
           
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Filtered = _SearchText;
           

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await iRecruitmentAppService.GetAll(_filter),
                async (result) =>
                {
                    var recruitmentPost = result.Items.ToList();
                    foreach (var item in recruitmentPost)
                    {
                        item.Recruiter.AvatarUrl = await articleService.GetPicture(item.Recruiter.AvatarUrl);
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
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(recruitmentPost, recruitmentPost.Count);
                    await UserDialogsService.UnBlock();
                    //await GetUserPhoto();

                }
            );

            return recruitmentDto;
        }
        public async Task ViewRecruitment(RecruitmentDto recruitment)
        {
            navigationService.NavigateTo($"ChiTietBTD?Id={recruitment.Id}&Experiences={recruitment.Experiences.DisplayName}&JobCatUnitName={recruitment.JobCatUnitName}&HumanResSizeCat={recruitment.Recruiter.HumanResSizeCat.DisplayName}");
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
        private CreateRecruitmentModal createRecruitmentModal { get; set; }
        public async Task OpenCreateModal()
        {
            await createRecruitmentModal.OpenFor(null);
        }

        private UpdateRecruimentModal updateRecruitmentModal { get; set; }
        public async Task UpdatePost(RecruitmentDto recruitmentDto)
        {
            await updateRecruitmentModal.OpenFor(recruitmentDto);
        }
        public async Task DeletePost(RecruitmentDto recruitmentDto)
        {
            var Isdelete = await UserDialogsService.Confirm("Bạn chắc muốn xoá tin tuyển dụng?", "Xóa", "Huỷ");
            if (Isdelete == true)
            {
                await iRecruitmentAppService.DeleteDoc(recruitmentDto.Id);
                await UserDialogsService.AlertSuccess(L("Xoá thành công " + recruitmentDto.Title));
                await RecruitmentContrainer.RefreshDataAsync();
                await RefeshList();
            }
            else
            {

            }

        }
        private async void DisPlayAction(RecruitmentDto recruitmentDto)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Sửa tin tuyển dụng", "Xóa");
            if (response == "Sửa tin tuyển dụng")
            {
                await UpdatePost(recruitmentDto);
            }
            else if (response == "Xóa")
            {
                await Notify(recruitmentDto);
                //await iRecruitmentAppService.DeleteDoc(recruitmentDto.Id);
                //await UserDialogsService.AlertSuccess(L("Xoá thành công " + recruitmentDto.Title));
                //await RefeshList();
            }
        }
        private async Task Notify(RecruitmentDto recruitmentDto)
        {
            var response = await App.Current.MainPage.DisplayAlert("Xác nhận", "Bạn chắc muốn xoá tin tuyển dụng?", "Xoá", "Huỷ");
            if (response)
            {
                await iRecruitmentAppService.DeleteDoc(recruitmentDto.Id);
                await UserDialogsService.AlertSuccess(L("Xoá thành công " + recruitmentDto.Title));
                await RecruitmentContrainer.RefreshDataAsync();
                StateHasChanged();


            }
            else
            {
                // await UserDialogsService.AlertSuccess(L("Xoá không thành công "));
                await LoadRecruitmentPost(new ItemsProviderRequest());
            }

        }
    }
}
