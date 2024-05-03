using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System.Runtime.CompilerServices;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class BaiUngTuyen : SaaSMainLayoutPageComponentBase
    {
        protected NavMenu NavMenu { get; set; }

        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IUserProfileService UserProfileService { get; set; }

        protected INavigationService navigationService { get; set; }
        private string _SearchText = "";
        private bool isError = false;
        private bool _IsCancelList;

        private ItemsProviderResult<CreateOrEditJobModel> jobApplicationDto;
        private readonly JobAppSearch _filter = new JobAppSearch();
        public BaiUngTuyen()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await LoadJobApplication(new ItemsProviderRequest());
        }
        private async Task RefeshList()
        {
            _IsCancelList = true;
            _SearchText = _filter.Search;
            await JobApplicationContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadJobApplication(new ItemsProviderRequest());
        }
        private Virtualize<CreateOrEditJobModel> JobApplicationContainer { get; set; }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            await JobApplicationContainer.RefreshDataAsync();
            StateHasChanged();

        }
        string _userImage;
        private async ValueTask<ItemsProviderResult<CreateOrEditJobModel>> LoadJobApplication(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            _filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = _SearchText;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await jobApplicationAppService.GetListJobAppOfCandidate(_filter),
                async (result) =>
                {
                    var jobFilter = ObjectMapper.Map<List<CreateOrEditJobModel>>(result.Items);
                    _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
                    if (jobFilter.Count == 0)
                    {
                        isError = true;
                    }
                    else
                    {
                        isError = false;
                    }
                    jobApplicationDto = new ItemsProviderResult<CreateOrEditJobModel>(jobFilter, jobFilter.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return jobApplicationDto;
        }

        public async Task ViewUser(CreateOrEditJobModel jobApplication)
        {
            navigationService.NavigateTo($"ChiTietBUT?Id={jobApplication.Id}");
        }

        #region Create/Edit
        private CreateOrEditJobModal createOrEditJobModal { get; set; }
        public async Task EditUser(CreateOrEditJobModel user)
        {
            await createOrEditJobModal.OpenForEdit(user);
        }
        public async Task OpenCreateModal()
        {
            await createOrEditJobModal.OpenFor(null);
        }

        public async Task DeleteJobApplication(CreateOrEditJobModel user)
        {
            var Isdelete = await UserDialogsService.Confirm("Bạn có chắc chắn xóa bài ứng tuyển không", "Xóa bài ứng tuyển");
            if (Isdelete == true) { 
                await jobApplicationAppService.DeleteJobApplication(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = user.Id });
                await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                await RefeshList();
            }
            else
            {
            }
        }
        private async void DisPlayAction(CreateOrEditJobModel CreateOrEditJobModel)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null ,  null, "Xem hồ sơ", "Sửa hồ sơ", "Xóa");
            if (response == "Xem hồ sơ")
            {
                await ViewUser(CreateOrEditJobModel);
            }
            else if (response == "Sửa hồ sơ")
            {
                await EditUser(CreateOrEditJobModel);
            }
            else if (response == "Xóa")
            {

               var Isdelete = await UserDialogsService.Confirm("Bạn có chắc chắn xóa bài ứng tuyển không", "Xóa bài ứng tuyển");

                if(Isdelete == true)
                {
                    await jobApplicationAppService.DeleteJobApplication(new Abp.Application.Services.Dto.NullableIdDto<long> { Id = CreateOrEditJobModel.Id });
                    await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                    await RefeshList();
                }
                else
                {
                    DisPlayAction(CreateOrEditJobModel);
                }
               
            }
        }
        #endregion
    }
}
