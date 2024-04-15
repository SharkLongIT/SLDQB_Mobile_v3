using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.InforNLD;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class ChiTietCVDUT : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected IApplicationRequestAppService applicationRequestAppService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        private bool IsUserLoggedIn;
        private string _userImage;

        [Parameter]
        public int Id { get; set; } = 0;
        public string Positions;
        public string FormOfWork;
        public string Literacy;
        //private ItemsProviderResult<GetJobApplicationForEditOutput> jobApplication;

        protected ThongTinHoSoUTModel Model = new();

        public ChiTietCVDUT()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            applicationRequestAppService = DependencyResolver.Resolve<IApplicationRequestAppService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Chi tiết bài ứng tuyển"), new List<Services.UI.PageHeaderButton>());

            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ChiTietCVDUT") + "ChiTietCVDUT".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            int? candidateId = null;

            if (q1["Id"] != null)
            {
                candidateId = int.Parse(q1["Id"]);
            }
            try
            {
                ApplicationRequestEditDto applicationRequest = await applicationRequestAppService.GetById(new NullableIdDto<long> { Id = candidateId });
                Model = new ThongTinHoSoUTModel();
                Model.JobApplication = new JobApplicationEditDto();
               // Model.Candidate = new CandidateEditDto();
                Model.User = new UserEditDto();
                Model.Recruitment = new Mdls.Profile.Recruiters.Dto.RecruitmentDto();
                //Thông tin cá nhân (account)
                Model.ProfilePictureId = applicationRequest.ProfilePictureId;
                Model.User.Id = applicationRequest.User.Id;
                Model.User.UserName = applicationRequest.User.UserName;
                Model.User.Surname = applicationRequest.User.Surname;
                Model.User.Name = applicationRequest.User.Name;
                Model.User.EmailAddress = applicationRequest.User.EmailAddress;
                Model.User.PhoneNumber = applicationRequest.User.PhoneNumber;
                Model.Content = applicationRequest.Content;

                Model.Candidate = applicationRequest.JobApplication.Candidate;
                // Thông tin người tìm việc
                //Model.Candidate.Id = candidate.JobApplication.Candidate.Id;
                //Model.CandidateAddress = applicationRequest.JobApplication.Candidate.Address;
                ////Model.Candidate.AvatarUrl = candidate.Candidate.AvatarUrl;
                //Model.CandidateDateOfBirth = applicationRequest.JobApplication.Candidate.DateOfBirth;
                ////Model.Candidate.PhoneNumber = candidate.Candidate.PhoneNumber;
                //Model.CandidateMarital = applicationRequest.JobApplication.Candidate.Marital;
                //Model.CandidateGender = applicationRequest.JobApplication.Candidate.Gender;

                //Thông tin Job
                Model.JobApplication.Id = candidateId;
                Model.JobApplication.CreationTime = applicationRequest.JobApplication.CreationTime;
                Model.JobApplication.LastModificationTime = applicationRequest.JobApplication.LastModificationTime;
                Model.JobApplication.Career = applicationRequest.JobApplication.Career;
                Model.Positions = applicationRequest.JobApplication.Positions.DisplayName;
                //Model.JobApplication.CurrencyUnit = candidate.JobApplication.CurrencyUnit;
                Model.JobApplication.DesiredSalary = applicationRequest.JobApplication.DesiredSalary;
                Model.Literacy = applicationRequest.JobApplication.Literacy.DisplayName;
                //Model.JobApplication.WorkExperiences = candidate.JobApplication.WorkExperiences;
                Model.FormOfWork = applicationRequest.JobApplication.FormOfWork.DisplayName;
                //Model.JobApplication.LearningProcessDtos = candidate.JobApplication.LearningProcessDtos;
                Model.JobApplication.Word = applicationRequest.JobApplication.Word;
                Model.JobApplication.PowerPoint = applicationRequest.JobApplication.PowerPoint;
                Model.JobApplication.Excel = applicationRequest.JobApplication.Excel;

                Model.Content = applicationRequest.Content;
                //Kinh nghiệm làm việc

                //Model.JobApplication.WorkExperiences = candidate.JobApplication.WorkExperiences;

                Model.JobApplication.WorkExperiences = new List<WorkExperienceEditDto>();  

                if (applicationRequest.JobApplication.WorkExperiences != null)
                {
                    Model.JobApplication.WorkExperiences.AddRange(applicationRequest.JobApplication.WorkExperiences);
                }
                Model.JobApplication.LearningProcess = new List<LearningProcessEditDto>();  

                if (applicationRequest.JobApplication.LearningProcess != null)
                {
                    Model.JobApplication.LearningProcess.AddRange(applicationRequest.JobApplication.LearningProcess);
                }
                await SetUserImageSourceAsync(Model);

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }


            //if (!candidate.ProfilePictureId.HasValue)
            //{
            //    candidate.Candidate.AvatarUrl = UserProfileService.GetDefaultProfilePicture();
            //    return;
            //}
            //candidate.Candidate.AvatarUrl = await UserProfileService.GetProfilePicture(candidate.Candidate.Id.Value);

            //Model.Candidate.AvatarUrl = candidate.Candidate.AvatarUrl;
        }
        private async Task SetUserImageSourceAsync(ThongTinHoSoUTModel nguoiTimViec)
        {
            if (ApplicationContext.LoginInfo == null)
            {
                nguoiTimViec.Photo = "media/default-profile-picture.png";
            }
            else
            {

                if (nguoiTimViec.Photo != null)
                {
                    return;
                }

                if (!nguoiTimViec.ProfilePictureId.HasValue)
                {
                    nguoiTimViec.Photo = UserProfileService.GetDefaultProfilePicture();
                    return;
                }

                nguoiTimViec.Photo = await UserProfileService.GetProfilePicture(nguoiTimViec.User.Id.Value);
            }
        }
    }
}