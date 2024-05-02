using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.InforNLD;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class UpdateInfoModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }

        protected IJobApplicationAppService JobApplicationAppService;
        protected ICandidateAppService CandidateAppService;
        protected IGeoUnitAppService GeoUnitAppService;

        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }

        protected CandidateModel Model = new();

        NguoiLaoDongModel candidateEditDto;
        protected IUserProfileService UserProfileService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }

        public UpdateInfoModal()
        {
            JobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            CandidateAppService = DependencyResolver.Resolve<ICandidateAppService>();
            GeoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();

            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }

        protected List<GeoUnitDto> _province;
        protected List<GeoUnitDto> _district;

        public string NameCandidate;
        public string NameMarital;
        public bool Marital;
        string _userImage;

        private List<GeoUnitDto> ListProvince { get; set; }
        private List<GeoUnitDto> ListAllGeoUnitDto { get; set; }
        private List<GeoUnitDto> ListDistrict { get; set; }
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

        public override string ModalId => "update-candidate";

        private async Task RefreshList()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            Model.ProvinceId = select;
            ListDistrict = ListAllGeoUnitDto.Where(x => x.ParentId == Model.ProvinceId).ToList();
            List<string> idList = new List<string>();
            StateHasChanged();
        }

        public async void ChangeGender(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            if (select == 1)
            {
                Model.Gender = GenderEnum.Male;
            }
            if (select == 2)
            {
                Model.Gender = GenderEnum.Female;
            }

        }

        public async void ChangeMarital(/*ChangeEventArgs args*/)
        {
            //if(Convert.ToBoolean(args.Value) == true)
            Model.Marital = !Model.Marital;
            if(Model.Marital == true)
            {
                NameMarital = "Độc thân";
            }else
            {
                NameMarital = "Đã kết hôn";
            }


        }

        private async Task<List<GeoUnitDto>> LoadProvince(long Province)
        {

            List<GeoUnitDto> geoUnitDtos = new List<GeoUnitDto>();
            await WebRequestExecuter.Execute(
                async () => await GeoUnitAppService.GetAll(),
                async (result) =>
                {
                    ListAllGeoUnitDto = result;
                    geoUnitDtos = result.Where(x => x.ParentId == null).ToList();
                    if(Province == null || Province == 0)
                    {
                        Model.ProvinceId = geoUnitDtos.FirstOrDefault().Id;
                    }
                    ListProvince = geoUnitDtos;
                    ListDistrict = ListAllGeoUnitDto.Where(x=>x.ParentId == Model.ProvinceId).ToList();
                    
                }
            );

            return ListProvince;
        }
        public async Task OpenFor(NguoiLaoDongModel candidateEditDto)
        {
            try
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                            Model = new CandidateModel();
                            _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
                            Model.NameCandidate = ApplicationContext.LoginInfo.User.Name;
                            NameCandidate = candidateEditDto.User.Name;
                            NameMarital = candidateEditDto.Candidate.Marital == true ? "Độc thân" : "Đã kết hôn";
                            Marital = candidateEditDto.Candidate.Marital;
                            Model = ObjectMapper.Map<CandidateModel>(candidateEditDto.Candidate);
                            await LoadProvince(candidateEditDto.Candidate.ProvinceId);
                        }
                    );

                });
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
           
            await Show();
        }

        private async Task<bool> ValidateInput()
        {
            if (Model.DateOfBirth.HasValue)
            {
                if (((int)(DateTime.Now - Model.DateOfBirth.Value).TotalDays / 365) < 16 || ((int)(DateTime.Now - Model.DateOfBirth.Value).TotalDays / 365) > 100)
                {
                    await UserDialogsService.AlertWarn(@L("Ngày sinh không hợp lệ"));
                    return false;
                }
            }  

            if (Model.ProvinceId == 0 || Model.ProvinceId == null )
            {
                await UserDialogsService.AlertWarn(@L("Tỉnh thành phố không được để trống"));
                return false;
            }
            if (Model.DistrictId == 0 || Model.DistrictId == null )
            {
                await UserDialogsService.AlertWarn(@L("Xã huyện không được để trống"));
                return false;
            }  

            if (string.IsNullOrEmpty(Model.Address))
            {
                await UserDialogsService.AlertWarn(@L("Địa chỉ không được để trống"));
                return false;
            }
            return true;

        }
        private async Task Save()
        {
            if(! await ValidateInput())
            {
                return;
            }
            await SetBusyAsync(async () =>
            {
                Model.District = null;
                Model.Province = null;
                await WebRequestExecuter.Execute(
                async () => await CandidateAppService.UpdateMobile(Model),
                async (result) =>
                {
                    await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                    await Hide();
                    await OnSave.InvokeAsync();
                }
               );
            });

        }
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
                        await CandidateAppService.UpdateProfilePictureFromMobile(imageBytes);

                    }

                }
            }
            else if (response == "Ảnh từ máy")
            {

                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    byte[] imageBytes;
                    var stream = await photo.OpenReadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                    await CandidateAppService.UpdateProfilePictureFromMobile(imageBytes);
                }

                this.StateHasChanged();

            }
            await RefreshList();
        }
        #endregion


    }
}