using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class ThongTinNTV : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }

        protected IUserProfileService UserProfileService { get; set; }
        protected IArticleService articleService { get; set; }

        [Parameter]
        public int Id { get; set; } = 0;
        public string Positions;
        public string FormOfWork;
        public string Literacy;
        private bool IsDefault1;
        //private ItemsProviderResult<GetJobApplicationForEditOutput> jobApplication;

        protected NguoiTimViecModal Model = new();

        public ThongTinNTV()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        private async Task RefeshList()
        {
            StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {
            if (ApplicationContext.LoginInfo == null || ApplicationContext.LoginInfo.User.UserType == Authorization.Users.UserTypeEnum.Type1)
            {
                IsDefault1 = true;
            }
            await SetPageHeader(L("Thông tin ứng viên"), new List<Services.UI.PageHeaderButton>());

            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ThongTinNTV") + "ThongTinNTV".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            int? candidateId = null;

            if (q1["Id"] != null)
            {
                candidateId = int.Parse(q1["Id"]);
            }
            if (q1["Positions"] != null)
            {
                Positions = (q1["Positions"]);
            }
            if (q1["FormOfWork"] != null)
            {
                FormOfWork = (q1["FormOfWork"]);
            }
            if (q1["Literacy"] != null)
            {
                Literacy = (q1["Literacy"]);
            }
            try
            {
                GetJobApplicationForEditOutput candidate = await jobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = candidateId });
                Model = new NguoiTimViecModal();
                Model.JobApplication = new JobApplicationEditDto();
                Model.Candidate = new CandidateEditDto();
                Model.User = new UserEditDto();
                //Thông tin cá nhân (account)
                Model.User.Id = candidate.User.Id;
              
                Model.User.UserName = candidate.User.UserName;
                Model.User.Surname = candidate.User.Surname;
                Model.User.Name = candidate.User.Name;
                Model.User.EmailAddress = candidate.User.EmailAddress;
                Model.User.PhoneNumber = candidate.User.PhoneNumber;

                // Thông tin người tìm việc
                Model.Candidate.Id = candidate.Candidate.Id;
                Model.Candidate.Address = candidate.Candidate.Address;
                //Model.Candidate.AvatarUrl = candidate.Candidate.AvatarUrl;
                if (candidate.Candidate.DateOfBirth != null)
                {
                     Model.Candidate.DateOfBirth = candidate.Candidate.DateOfBirth;
                }    
                //Model.Candidate.PhoneNumber = candidate.Candidate.PhoneNumber;
                Model.Candidate.Marital = candidate.Candidate.Marital;
                Model.Candidate.Gender = candidate.Candidate.Gender;
                if (candidate.Candidate.District != null)
                {
                      Model.DistrictName = candidate.Candidate.District.DisplayName;
                }
                if (candidate.Candidate.Province != null)
                {
                      Model.ProvinceName = candidate.Candidate.Province.DisplayName;
                }

                //Thông tin Job
                Model.JobApplication.Id = candidateId;
                if (candidate.JobApplication.Experiences != null)
                {
                 Model.Experience = candidate.JobApplication.Experiences.DisplayName;
                }
                Model.JobApplication.CreationTime = candidate.JobApplication.CreationTime;
                if (candidate.JobApplication.Occupations != null)
                {
                      Model.Occupation = candidate.JobApplication.Occupations.DisplayName;
                }
                if (candidate.JobApplication.Province != null)
                {
                      Model.Province = candidate.JobApplication.Province.DisplayName;
                }
                Model.JobApplication.LastModificationTime = candidate.JobApplication.LastModificationTime;

                Model.JobApplication.Career = candidate.JobApplication.Career;
                Model.Positions = Positions;
                //Model.JobApplication.CurrencyUnit = candidate.JobApplication.CurrencyUnit;
                Model.JobApplication.DesiredSalary = candidate.JobApplication.DesiredSalary;
                Model.Literacy = Literacy;
                //Model.JobApplication.WorkExperiences = candidate.JobApplication.WorkExperiences;
                Model.FormOfWork = FormOfWork;
                //Model.JobApplication.LearningProcessDtos = candidate.JobApplication.LearningProcessDtos;
                Model.JobApplication.Word = candidate.JobApplication.Word;
                Model.JobApplication.PowerPoint = candidate.JobApplication.PowerPoint;
                Model.JobApplication.Excel = candidate.JobApplication.Excel;


                //Kinh nghiệm làm việc
              
                //Model.JobApplication.WorkExperiences = candidate.JobApplication.WorkExperiences;

                Model.JobApplication.WorkExperiences = new List<WorkExperienceEditDto>();  

                if (candidate.JobApplication.WorkExperiences != null &&  candidate.JobApplication.WorkExperiences.Count > 0)
                {
                    Model.JobApplication.WorkExperiences.AddRange(candidate.JobApplication.WorkExperiences);
                }
                Model.JobApplication.LearningProcess = new List<LearningProcessEditDto>();  
                if (candidate.JobApplication.LearningProcess != null)
                {
                    Model.JobApplication.LearningProcess.AddRange(candidate.JobApplication.LearningProcess);
                }
                //await UpdateImage();
                await SetUserImageSourceAsync(Model);


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
           

        }
        private async Task SetUserImageSourceAsync(NguoiTimViecModal nguoiTimViec)
        {
           Model.Photo =  await UserProfileService.GetProfilePicture(nguoiTimViec.User.Id.Value); ;
        }
        private DatLichPVModal datLichPVModal { get; set; }
        public async Task BookUser(NguoiTimViecModal nTDDatLichModel)
        {
            if (ApplicationContext.LoginInfo == null)
            {
                await UserDialogsService.AlertWarn("Vui lòng đăng nhập để đặt lịch!");
                //navigationService.NavigateTo(NavigationUrlConsts.Login);
            }
            else
            {
                await datLichPVModal.OpenFor(nTDDatLichModel);
            }
        }

        bool IsExpView = false;
        bool IsLearnView = false;
        #region Exp
        public async Task ViewAllExp()
        {
            IsExpView = true;
            StateHasChanged();
        }
        public async Task CloseExpView()
        {
            IsExpView = false;
            StateHasChanged();
        }
        #endregion

        #region Learn
        public async Task ViewAllLearn()
        {
            IsLearnView = true;
            StateHasChanged();
        }
        public async Task CloseLearnView()
        {
            IsLearnView = false;
            StateHasChanged();
        }
        #endregion
    }
}
