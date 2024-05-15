using BBK.SaaS.ApiClient;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec;
using BBK.SaaS.NguoiTimViec;
using Microsoft.AspNetCore.Components;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.ViecTimNguoi;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class DanhSachUVDUT : SaaSMainLayoutPageComponentBase
    {
        protected INavigationService navigationService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }

        protected IApplicationRequestAppService applicationRequestAppService { get; set; }
        private string _SearchText = "";
        private bool isError = false;
        private int? _Status;
        private long? _Rank;
        private int? _Experience;
        private bool _IsCancelList;

        private ItemsProviderResult<ApplicationRequestEditDto> applicationRequestDto;
        private readonly ApplicationRequestSearch _filter = new ApplicationRequestSearch();
        private Virtualize<ApplicationRequestEditDto> ApplicationRequestContainer { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }

        protected IGeoUnitAppService geoUnitAppService { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        public GeoUnitDto geoUnitDtos { get; set; }
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }

        private bool IsUserLoggedIn;
        private string _userImage;
        private List<CatUnitDto> _experience { get; set; }

        public List<CatUnitDto> Experience
        {
            get => _experience;
            set => _experience = value;
        }

        public DanhSachUVDUT()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            applicationRequestAppService = DependencyResolver.Resolve<IApplicationRequestAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();

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
            if (_filter.Experience.HasValue)
            {
                _Experience = _filter.Experience.Value;
            }
            else
            {
                _Experience = null;
            }
            if (_filter.Status.HasValue)
            {

                _Status = _filter.Status.Value;
            }
            else
            {
                _Status = null;
            }
            //_Rank = _filter.Rank.Value;
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
        #region

        public CatFilterList catFilterList { get; set; }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {
                 _experience = catUnit.Experience;

                 var filterList = new CatFilterList()
                 {
                     Experience = _experience,

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

        private async ValueTask<ItemsProviderResult<ApplicationRequestEditDto>> LoadApplicationRequest(ItemsProviderRequest request)
        {

            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = _SearchText;
            _filter.Experience = _Experience;
            //_filter.Rank = _Rank;
            _filter.Status = _Status;

            await UserDialogsService.Block();

             await WebRequestExecuter.Execute(
                async () => await applicationRequestAppService.GetAllByRecruiter(_filter),
                async (result) =>
                {
                    var recruitmentPost = result.Items.OrderByDescending(item => item.CreationTime).ToList();
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
            GetJobApplicationForEditOutput candidate = await jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = applicationRequest.JobApplicationId });

            navigationService.NavigateTo($"ThongTinNTV?Id={applicationRequest.JobApplicationId}&Positions={candidate.JobApplication.Positions.DisplayName}&FormOfWork={candidate.JobApplication.FormOfWork.DisplayName}&Literacy={candidate.JobApplication.Literacy.DisplayName}");

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
        private DatLich datLich { get; set; }
        public async Task BookUser(ApplicationRequestEditDto applicationRequest)
        {
                await datLich.OpenFor(applicationRequest);
        }
     
        public async Task DeleteAppRequest(ApplicationRequestEditDto applicationRequest)
        {
            var Isdelete = await UserDialogsService.Confirm("Bạn chắc muốn xoá ?", "Xóa", "Huỷ");
            if (Isdelete == true)
            {
                await applicationRequestAppService.Delete(applicationRequest.Id);
                await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                await ApplicationRequestContainer.RefreshDataAsync();
                StateHasChanged();
            }
            else
            {

            }
        }

        private async void DisPlayAction(ApplicationRequestEditDto applicationRequest)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Xem CV ứng viên", "Đặt lịch", "Xóa");
            if (response == "Đặt lịch")
            {
                await BookUser(applicationRequest);
            }
            else if (response == "Xóa")
            {
                await applicationRequestAppService.Delete(applicationRequest.Id);
                await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                await ApplicationRequestContainer.RefreshDataAsync();
                StateHasChanged();

            }
            else
            {
                await ViewApplication(applicationRequest);
            }
        }
    }
}
