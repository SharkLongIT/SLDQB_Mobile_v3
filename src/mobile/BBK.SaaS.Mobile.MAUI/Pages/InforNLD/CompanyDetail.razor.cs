using Abp.Application.Navigation;
using Abp.Application.Services.Dto;
using Abp.Threading;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NhaTuyenDung;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.ProfileNTD;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class CompanyDetail : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IArticleService articleService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IRecruiterAppService recruiterAppService { get; set; }

        private readonly RecruimentInput _filter = new RecruimentInput();
        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private Virtualize<RecruitmentDto> RecruitmentContainer { get; set; }

        [Parameter]
        public long Id { get; set; } = 0;
        //protected ViecTimNguoiModel Model = new ViecTimNguoiModel();
        protected NhaTuyenDungModel Model = new NhaTuyenDungModel();
        private DateTime date;
        public string HumanResSizeCat;
        public string Experiences;
        private MarkupString htmlContent1;
        private MarkupString htmlContent2;
        private MarkupString htmlContent3; 
        public string Description;
        public bool IsShowDes;

        public CompanyDetail()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            recruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await SetPageHeader(L("Thông tin công ty"), new List<Services.UI.PageHeaderButton>());
                var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("CompanyDetail") + "CompanyDetail".Length);
                var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
                //int? Id = null;

                if (q1["RecruiterId"] != null)
                {
                    Id = int.Parse(q1["RecruiterId"]);
                }
                GetRecruiterForEditOutput recruiter = await recruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = Id });
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
                //Model.Recruiter.Description = recruiter.Recruiter.Description;
                if (recruiter.Recruiter.Description.Length <= 120)
                {
                    Description = recruiter.Recruiter.Description;
                }
                else
                {
                 Description = recruiter.Recruiter.Description.Substring(0, 120) + "...";
                }
                Model.Recruiter.ImageCoverUrl = recruiter.Recruiter.ImageCoverUrl;
                Model.Recruiter.CompanyName = recruiter.Recruiter.CompanyName;
                Model.Recruiter.UserId = recruiter.Recruiter.UserId;
                Model.Recruiter.ContactName = recruiter.Recruiter.ContactName;
                Model.Recruiter.ContactEmail = recruiter.Recruiter.ContactEmail;
                Model.Recruiter.ContactPhone = recruiter.Recruiter.ContactPhone;
                Model.HumanResSizeCatName = recruiter.Recruiter.HumanResSizeCat.DisplayName; // quy mô nhân sự
                Model.SphereOfActivityName = recruiter.Recruiter.SphereOfActivity.DisplayName; // lĩnh vực hoạt động
                Model.Recruiter.WebSite = recruiter.Recruiter.WebSite;
                Model.Recruiter.TaxCode = recruiter.Recruiter.TaxCode; // mã số thuế
                date = recruiter.Recruiter.DateOfEstablishment.Value; // ngày thành lập công ty
                Model.Recruiter.Address = recruiter.Recruiter.Address;
                //Model.Recruiter.ProvinceId = recruiter.Recruiter.ProvinceId; // thành phố
                Model.Province = recruiter.Recruiter.Province.DisplayName;
                //Model.Recruiter.DistrictId = recruiter.Recruiter.DistrictId; // quận huyện
                Model.District = recruiter.Recruiter.District.DisplayName;
                //Model.Recruiter.VillageId = recruiter.Recruiter.VillageId; // xã phường
                Model.Village = recruiter.Recruiter.Village.DisplayName;
                Model.Recruiter.Description = recruiter.Recruiter.Description;
                Model.AvatarUrl = recruiter.Recruiter.AvatarUrl;

                await UpdateImage();
                await LoadRecruitment(new ItemsProviderRequest());
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
         



        }
        public Task UpdateImage()
        {
            Model.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.AvatarUrl));
            Model.Recruiter.ImageCoverUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.Recruiter.ImageCoverUrl));
            return Task.CompletedTask;
        }
        //public async Task ViewCompany(RecruitmentDto recruitmentDto)
        //{
        //    navigationService.NavigateTo($"ThongTinCongTy?Id={recruitmentDto.RecruiterId}");
        //}
        private int Count;
        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitment(ItemsProviderRequest request)
        {

            //_filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            //_filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            //_filter.Filtered = _SearchText;


            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await recruitmentAppService.GetAllByAllUser(_filter),
                async (result) =>
                {
                    var recruitmentPost = result.Items.Where(x => x.Recruiter.UserId == Model.Recruiter.UserId).ToList();
                    foreach (var item in recruitmentPost)
                    {
                        item.Recruiter.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(item.Recruiter.AvatarUrl));

                    }
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(recruitmentPost, recruitmentPost.Count);
                    Count = recruitmentPost.Count;
                    await UserDialogsService.UnBlock();
                }
            );

            return recruitmentDto;
        }

        public async Task ViewRecruitment(RecruitmentDto recruitment)
        {
            navigationService.NavigateTo($"ThongTinVTN?Id={recruitment.Id}&HumanResSizeCat={recruitment.Recruiter.HumanResSizeCat.DisplayName}&Experiences={recruitment.Experiences.DisplayName}&SphereOfActivity={recruitment.Recruiter.SphereOfActivity.DisplayName}");
            await OnInitializedAsync();
        }


        
        #region Change Des
        private async Task ChangeDes()
        {
            IsShowDes = !IsShowDes;
            StateHasChanged();
        }
        private async Task Collaspe()
        {
            IsShowDes = false;
            StateHasChanged();
        }
        #endregion
    }
}
