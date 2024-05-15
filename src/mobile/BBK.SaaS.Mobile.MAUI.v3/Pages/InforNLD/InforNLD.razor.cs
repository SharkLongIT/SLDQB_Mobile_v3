using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Authorization.Users.Profile;
using BBK.SaaS.Authorization.Users.Profile.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class InforNLD : SaaSMainLayoutPageComponentBase
    {

        //protected IJobApplicationAppService jobaplicationAppService { get; set; }
        
        protected ICandidateAppService candidateAppService { get; set; }
        protected IGeoUnitAppService geoUnitAppService { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IProfileAppService ProfileAppService { get; set; }
        private bool IsUserLoggedIn;
        private string _userImage;

        protected NguoiLaoDongModel Model = new();

        protected List<GeoUnitDto> _province;
        protected List<GeoUnitDto> _district;


        public List<GeoUnitDto> Province
        {
            get => _province;
            set => _province = value;
        }
        public List<GeoUnitDto> District
        {
            get => _district;
            set => _district = value;
        }


        public InforNLD()
        {
            candidateAppService = DependencyResolver.Resolve<ICandidateAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();

            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            ProfileAppService = DependencyResolver.Resolve<IProfileAppService>();

        }
        private DateTime dateOfBirth;
        private async Task RefreshList()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsUserLoggedIn = navigationService.IsUserLoggedIn();
                await SetPageHeader(L("Thông tin cá nhân"), new List<Services.UI.PageHeaderButton>());
                GetCandidateForEditOutput candidate = await candidateAppService.GetCandidateForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });

                Model = new NguoiLaoDongModel();
                Model.Candidate = new CandidateEditDto();
                Model.User = new UserEditDto();
                //Thông tin cá nhân (account)
                Model.User.Id = candidate.User.Id;
                Model.User.UserName = candidate.User.UserName;
                Model.User.Surname = candidate.User.Surname;
                Model.User.Name = candidate.User.Name;
                Model.User.EmailAddress = candidate.User.EmailAddress;
                Model.User.PhoneNumber = candidate.User.PhoneNumber;

                //// Thông tin người tìm việc
                Model.Candidate.Id = candidate.Candidate.Id;
                Model.Candidate.Address = candidate.Candidate.Address;
                Model.Candidate.AvatarUrl = candidate.Candidate.AvatarUrl;
                Model.Candidate.DateOfBirth = candidate.Candidate.DateOfBirth;
                if (candidate.Candidate.DateOfBirth.HasValue)
                {

                dateOfBirth = candidate.Candidate.DateOfBirth.Value;
                }
                //Model.Candidate.PhoneNumber = candidate.Candidate.PhoneNumber;
                Model.Candidate.Marital = candidate.Candidate.Marital;
                Model.Candidate.Gender = candidate.Candidate.Gender;

                Model.Province = candidate.Candidate.Province.DisplayName;
                Model.District = candidate.Candidate.District.DisplayName;

                Model.Candidate.ProvinceId = candidate.Candidate.ProvinceId;
                Model.Candidate.DistrictId = candidate.Candidate.DistrictId;
                Model.Candidate.Province = candidate.Candidate.Province;
                Model.Candidate.District = candidate.Candidate.District;
                Model.Candidate.UserId = candidate.Candidate.UserId;
                Model.Candidate.TenantId= candidate.Candidate.TenantId;
               // Model.Candidate.AvatarUrl = "aa";
                var geo = await geoUnitAppService.GetGeoUnits();
                _province = geo.Items.Where(x => x.ParentId == null).ToList();
                _district = geo.Items.Where(x => x.ParentId == candidate.Candidate.ProvinceId).ToList();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
            await GetUserPhoto();
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



        private async Task UpdateCandidate()
        {

			await SetBusyAsync(async () =>
			{
				await WebRequestExecuter.Execute(
				  async () => await candidateAppService.Update(Model.Candidate),
				  async (result) =>
				  {
                      var output = result;
					  await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
				  }
			   );
			});
            StateHasChanged();
        }

        #region Create/Edit
        private UpdateInfoModal UpdateInfoModal { get; set; }
        public async Task EditUser()
        {
            await UpdateInfoModal.OpenFor(Model);
        }
        //public async Task OpenCreateModal(Model)
        //{
        //    await UpdateInfoModal.OpenFor(null);
        //}
        #endregion

        #region Change Avatar 
        private async void ChangeAvatar()
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Chụp ảnh", "Ảnh từ máy");
            if (response == "Chụp ảnh")
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        byte[] imageBytes;
                        var stream = await photo.OpenReadAsync();
                        using (MemoryStream ms = new MemoryStream())
                        {
                            stream.CopyTo(ms);
                            imageBytes = ms.ToArray();
                        }
                        await candidateAppService.UpdateProfilePictureFromMobile(imageBytes);

                    }
                  
                }
            }
            else if (response == "Ảnh từ máy")
            {
               
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if(photo != null)
                {
                    byte[] imageBytes;
                    var stream = await photo.OpenReadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                    await candidateAppService.UpdateProfilePictureFromMobile(imageBytes);
                }
                
                this.StateHasChanged();
              
            }
            await RefreshList();
        }
        #endregion

        #region show Modal
        private TestModal TestModal { get; set; }
        private async Task ShowModal()
        {
            await TestModal.OpenFor(Model);
            // JS.InvokeAsync()
        }
		#endregion
	}
}
