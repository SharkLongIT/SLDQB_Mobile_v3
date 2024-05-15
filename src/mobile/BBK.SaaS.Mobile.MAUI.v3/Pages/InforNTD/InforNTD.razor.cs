using Abp.Application.Services.Dto;
using Abp.Threading;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NhaTuyenDung;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class InforNTD : SaaSMainLayoutPageComponentBase
    {
        protected NavMenu NavMenu { get; set; }
        protected IRecruiterAppService recruiterAppService { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected ICandidateAppService candidateAppService { get; set; }
        protected IGeoUnitAppService geoUnitAppService { get; set; }
        protected ICatUnitAppService catUnitAppService { get; set; }

        protected INavigationService navigationService { get; set; }

        protected IUserProfileService UserProfileService { get; set; }
        protected IAccessTokenManager AccessTokenManager { get; set; }

        protected IArticleService articleService { get; set; }

        private bool IsUserLoggedIn;

        protected NhaTuyenDungModel Model = new();


        protected IApplicationContext ApplicationContext { get; set; }

        private ItemsProviderResult<GetRecruiterForEditOutput> recruiterDto;
        private DateTime date;
        private DateTime dateOfBirth;

        private List<GeoUnitDto> _geoUnitDtos;
        private List<GeoUnitDto> _district;
        private List<GeoUnitDto> _village;
        private List<CatUnitDto> _humanResSize;
        private CatFilterList catFilterList;

        public InforNTD()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            candidateAppService = DependencyResolver.Resolve<ICandidateAppService>();
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            catUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            articleService = DependencyResolver.Resolve<IArticleService>();


        }


        private string _userImage;

        public List<GeoUnitDto> GeoUnitDtos
        {
            get => _geoUnitDtos;
            set => _geoUnitDtos = value;
        }
        public List<GeoUnitDto> District
        {
            get => _district;
            set => _district = value;
        }
        public List<GeoUnitDto> Village
        {
            get => _village;
            set => _village = value;
        }

        public List<CatUnitDto> HumanResSize
        {
            get => _humanResSize;
            set => _humanResSize = value;
        }
        public CatFilterList CatFilterList
        {
            get => catFilterList;
            set => catFilterList = value;
        }

        public string Description;
        public bool IsShowDes;

        private async Task RefreshList()
        {
            await OnInitializedAsync();
            StateHasChanged();
        }

        protected override async Task OnInitializedAsync()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();
            //if (ApplicationContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
            //{
            await SetPageHeader(L("Thông tin cá nhân"), new List<Services.UI.PageHeaderButton>());
            GetRecruiterForEditOutput recruiter = await recruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });
            Model = new NhaTuyenDungModel();
            //Model.ProfilePictureId = new Guid(Guid.NewGuid().ToString());
            //Model.ProfilePictureId = recruiter?.ProfilePictureId;
            Model.User = new UserEditDto();
            Model.Recruiter = new RecruiterEditDto();

            //Thông tin đăng nhập
            Model.User.Id = recruiter.User.Id;
            Model.User.UserName = recruiter.User.UserName;
            Model.User.Surname = recruiter.User.Surname;
            Model.User.Name = recruiter.User.Name;
            Model.User.PhoneNumber = recruiter.User.PhoneNumber;
            Model.User.EmailAddress = recruiter.User.EmailAddress;

            //Thông tin tuyển dụng
            Model.Recruiter.CompanyName = recruiter.Recruiter.CompanyName;
            Model.Recruiter.ContactName = recruiter.Recruiter.ContactName;
            Model.Recruiter.ContactEmail = recruiter.Recruiter.ContactEmail;
            Model.Recruiter.ContactPhone = recruiter.Recruiter.ContactPhone;
            Model.HumanResSizeCatName = recruiter.Recruiter.HumanResSizeCat.DisplayName; // quy mô nhân sự
            Model.SphereOfActivityName = recruiter.Recruiter.SphereOfActivity.DisplayName; // lĩnh vực hoạt động
            Model.Recruiter.WebSite = recruiter.Recruiter.WebSite;
            Model.Recruiter.TaxCode = recruiter.Recruiter.TaxCode; // mã số thuế
            date = recruiter.Recruiter.DateOfEstablishment.Value; // ngày thành lập công ty
            Model.Recruiter.Address = recruiter.Recruiter.Address;
            Model.Recruiter.ProvinceId = recruiter.Recruiter.ProvinceId; // thành phố
            Model.Province = recruiter.Recruiter.Province.DisplayName;
            Model.Recruiter.DistrictId = recruiter.Recruiter.DistrictId; // quận huyện
            Model.District = recruiter.Recruiter.District.DisplayName;
            Model.Recruiter.VillageId = recruiter.Recruiter.VillageId; // xã phường
            Model.Village = recruiter.Recruiter.Village.DisplayName;
            Model.Recruiter.Description = recruiter.Recruiter.Description;
            Model.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(recruiter.Recruiter.ImageCoverUrl));
            if (recruiter.Recruiter.Description.Length <= 30)
            {
                Description = recruiter.Recruiter.Description;
            }
            else
            {
                Description = recruiter.Recruiter.Description.Substring(0, 30) + "...";
            }
            Model.Recruiter.UserId = recruiter.Recruiter.UserId;
            Model.Recruiter.HumanResSizeCatId = recruiter.Recruiter.HumanResSizeCatId;
            Model.Recruiter.DateOfEstablishment = recruiter.Recruiter.DateOfEstablishment;
            Model.Recruiter.Id = recruiter.Recruiter.Id;
            Model.Recruiter.AvatarUrl = recruiter.Recruiter.AvatarUrl;
            Model.Recruiter.BusinessLicenseUrl = recruiter.Recruiter.BusinessLicenseUrl;
            Model.Recruiter.SphereOfActivityId = recruiter.Recruiter.SphereOfActivityId;

            // Địa điểm 
            var geoUnit = await geoUnitAppService.GetGeoUnits();
            if (geoUnit != null)
            {
                GeoUnitDtos = geoUnit.Items.Where(x => x.ParentId == null).ToList();
            }
           // await LoadDistrict(new ChangeEventArgs { Value = Model.Recruiter.ProvinceId });
            //await LoadVillage(new ChangeEventArgs { Value = Model.Recruiter.DistrictId });
            //catFilterList = catUnitAppService.GetFilterList().Result;

            //_humanResSize = catFilterList.StaffSize;
            //await LoadFillter();
            //}
            //else
            //{
            //    navigationService.NavigateTo(NavigationUrlConsts.InforNLD);

            //}
            await GetUserPhoto();

        }

        #region
        //private async Task LoadFillter()
        //{
        //    await WebRequestExecuter.Execute(
        //               async () => await catUnitAppService.GetFilterList(),
        //               async (catUnit) =>
        //               {
        //                   catFilterList = catUnit;
        //                   _humanResSize = catFilterList.StaffSize;
        //               }
        //           );
        //}

        //private async Task LoadDistrict(ChangeEventArgs args)
        //{

        //    var value = (long)args.Value;
        //    await WebRequestExecuter.Execute(
        //              async () => await geoUnitAppService.GetGeoUnits(),
        //              async (geoUnit) =>
        //              {
        //                  _district = geoUnit.Items.Where(x => x.ParentId == value).ToList();
        //              }
        //          );
        //}

        //private async Task LoadVillage(ChangeEventArgs args)
        //{

        //    var value = (long)args.Value;
        //    await WebRequestExecuter.Execute(
        //              async () => await geoUnitAppService.GetGeoUnits(),
        //              async (geoUnit) =>
        //              {
        //                  _village = geoUnit.Items.Where(x => x.ParentId == value).ToList();
        //              }
        //          );
        //}

        #endregion
        private async Task GetUserPhoto()
        {
            if (!IsUserLoggedIn)
            {
                return;
            }

            _userImage = await UserProfileService.GetProfilePicture(ApplicationContext.LoginInfo.User.Id);
            StateHasChanged();
        }

        #region Create/Edit
        private UpdateInforNTDModal UpdateInforNTDModal { get; set; }
        public async Task EditRecruiter()
        {
            await UpdateInforNTDModal.OpenFor(Model);
        }

        #endregion

        private async Task UpdateRecruiter()
        {
            await recruiterAppService.Update(Model.Recruiter);
            await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
            StateHasChanged();
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
                        await candidateAppService.UpdateProfilePictureFromMobile(imageBytes);

                    }

                }
            }
            else if (response == "Ảnh từ máy")
            {
                byte[] imageBytes;
                var photo = await MediaPicker.Default.PickPhotoAsync();
                if (photo != null)
                {
                    var stream = await photo.OpenReadAsync();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        stream.CopyTo(ms);
                        imageBytes = ms.ToArray();
                    }
                    await candidateAppService.UpdateProfilePictureFromMobile(imageBytes);
                }

            }
            await RefreshList();
        }
        #endregion

        #region Change Des
        private async Task ChangeDes()
        {
            IsShowDes = !IsShowDes;
            StateHasChanged();
        }
        #endregion
    }
}