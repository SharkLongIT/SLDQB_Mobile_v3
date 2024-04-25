using Abp.Threading;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class ThongTinVTN : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IArticleService articleService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }

        protected IRecruitmentAppService iRecruitmentAppService { get; set; }
        private readonly RecruimentInput _filter = new RecruimentInput();
        private ItemsProviderResult<RecruitmentDto> recruitmentDto;
        private Virtualize<RecruitmentDto> RecruitmentContrainer { get; set; }
        private bool IsDefault2;

        [Parameter]
        public long Id { get; set; } = 0;
        protected ViecTimNguoiModel Model = new ViecTimNguoiModel();
        private DateTime date;
        public string HumanResSizeCat;
        public string Experiences;
        public string SphereOfActivity;
        public string WorkAddress;
        private MarkupString htmlContent1;
        private MarkupString htmlContent2;
        private MarkupString htmlContent3;

        public ThongTinVTN()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            iRecruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                if (ApplicationContext.LoginInfo == null || ApplicationContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type2)
                {
                    IsDefault2 = true;
                }
                await SetPageHeader(L("Thông tin việc làm"), new List<Services.UI.PageHeaderButton>());
                var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ThongTinVTN") + "ThongTinVTN".Length);
                var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
                //int? Id = null;

                if (q1["Id"] != null)
                {
                    Id = int.Parse(q1["Id"]);
                }
                if (q1["HumanResSizeCat"] != null)
                {
                    HumanResSizeCat = (q1["HumanResSizeCat"]);
                }
                if (q1["Experiences"] != null)
                {
                    Experiences = (q1["Experiences"]);
                }
                if (q1["WorkAddress"] != null)
                {
                    WorkAddress = (q1["WorkAddress"]);
                }
                RecruitmentDto recruitment = await recruitmentAppService.GetDetail(Id);

                Model = new ViecTimNguoiModel();
                Model.Id = recruitment.Id;
                Model.Title = recruitment.Title;// Vị trí , tiêu đề
                Model.MinSalary = recruitment.MinSalary;
                Model.MaxSalary = recruitment.MaxSalary;
                Model.DeadlineSubmission = recruitment.DeadlineSubmission;

                Model.RecruiterId = recruitment.Recruiter.UserId;
                //company
                Model.CompanyName = recruitment.Recruiter.CompanyName;

                if (HumanResSizeCat != "" || HumanResSizeCat != null)
                {
                      Model.HumanResSizeCat = HumanResSizeCat; // Quy mô nhân sự
                }
                if (WorkAddress == null)
                {
                    Model.AddressForWork = recruitment.AddressName;
                }
                else
                {
                    Model.AddressForWork = WorkAddress;// địa chỉ làm việc
                }
                Model.AvatarUrl = recruitment.Recruiter.AvatarUrl;
                Model.ImageCoverUrl = recruitment.Recruiter.ImageCoverUrl;
                Model.AddressCompany = recruitment.Recruiter.Address; //địa chỉ công ty
                Model.Website = recruitment.Recruiter.WebSite;
                //Thông tin tuyển dụng:
                Model.JobCatUnitName = recruitment.JobCatUnitName;
                Model.AddressName = recruitment.AddressName;
                Model.NecessarySkills = recruitment.NecessarySkills;
                Model.Experience = Experiences; // kinh nghiệm
               
                Model.FormOfWork = recruitment.FormOfWork; // Hình thức làm việc
                Model.FormOfWorkName = recruitment.FormOfWorks.DisplayName;
                Model.NumberOfRecruits = recruitment.NumberOfRecruits; // số lượng
                Model.GenderRequired = recruitment.GenderRequired; // yc giới tính

                htmlContent1 = new MarkupString(recruitment.JobDesc);
                htmlContent2 = new MarkupString(recruitment.JobRequirementDesc);
                htmlContent3 = new MarkupString(recruitment.BenefitDesc);


                //người liên hệ
                Model.FullName = recruitment.FullName;
                Model.Email = recruitment.Email;
                Model.PhoneNumber = recruitment.PhoneNumber;
                Model.Address = recruitment.Address;
                await UpdateImage();
            }
            catch (Exception ex )
            {

                throw new UserFriendlyException(ex.Message);
            }
       


        }
        public Task UpdateImage()
        {
            Model.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.AvatarUrl));
            Model.ImageCoverUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.ImageCoverUrl));
            return Task.CompletedTask;
        }
        public async Task ViewCompany(ViecTimNguoiModel recruitmentDto)
        {
            navigationService.NavigateTo($"CompanyDetail?RecruiterId={recruitmentDto.RecruiterId}");
        }
     
            
        #region Ứng tuyển 
        private UngtuyenModal ungtuyenModal { get; set; }
        public async Task UngTuyen(ViecTimNguoiModel ViecTimNguoiModel)
        {
            if (ApplicationContext.LoginInfo == null)
            {
                await UserDialogsService.AlertWarn("Vui lòng đăng nhập để ứng tuyển!");
                //navigationService.NavigateTo(NavigationUrlConsts.Login);
            }
            else if (ApplicationContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
            {
                await UserDialogsService.AlertWarn("Vui lòng đăng nhập bằng tài khoản Người lao động để ứng tuyển!");
            }
            else
            {
                await ungtuyenModal.OpenFor(ViecTimNguoiModel);
            }
        }
        #endregion


        #region Company

        private async ValueTask<ItemsProviderResult<RecruitmentDto>> LoadRecruitmentPost(ItemsProviderRequest request)
        {

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                async () => await iRecruitmentAppService.GetAllByAllUser(_filter),
                async (result) =>
                {
                    var recruitmentPost = result.Items.Where(x => x.Recruiter.UserId == Model.RecruiterId && x.Id != Model.Id).ToList();
                    foreach (var item in recruitmentPost)
                    {
                        item.Recruiter.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(item.Recruiter.AvatarUrl));

                    }
                    recruitmentDto = new ItemsProviderResult<RecruitmentDto>(recruitmentPost, recruitmentPost.Count);
                    await UserDialogsService.UnBlock();
                }
            );

            return recruitmentDto;
        }

        #endregion
        public async Task ViewRecruitment(RecruitmentDto recruitment)
        {
            navigationService.NavigateTo($"ThongTinVTN?Id={recruitment.Id}&HumanResSizeCat={recruitment.Recruiter.HumanResSizeCat.DisplayName}&Experiences={recruitment.Experiences.DisplayName}&SphereOfActivity={recruitment.Recruiter.SphereOfActivity.DisplayName}");
            await OnInitializedAsync();
            await RecruitmentContrainer.RefreshDataAsync();
            StateHasChanged();
            //await LoadRecruitmentPost(new ItemsProviderRequest());
        }
    }
}
