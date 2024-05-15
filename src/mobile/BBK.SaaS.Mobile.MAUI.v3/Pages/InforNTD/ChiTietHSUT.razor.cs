using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.ApiClient;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class ChiTietHSUT : SaaSMainLayoutPageComponentBase
    {

        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }

        protected IUserProfileService UserProfileService { get; set; }

        [Parameter]
        public int Id { get; set; } = 0;
        public string Positions;
        public string FormOfWork;
        public string Literacy;
        //private ItemsProviderResult<GetJobApplicationForEditOutput> jobApplication;

        protected NguoiTimViecModal Model = new();

        public ChiTietHSUT()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Hồ sơ ứng tuyển"), new List<Services.UI.PageHeaderButton>());

            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ChiTietHSUT") + "ChiTietHSUT".Length);
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
                Model.ProfilePictureId = candidate.ProfilePictureId;
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
                Model.Candidate.DateOfBirth = candidate.Candidate.DateOfBirth;
                //Model.Candidate.PhoneNumber = candidate.Candidate.PhoneNumber;
                Model.Candidate.Marital = candidate.Candidate.Marital;
                Model.Candidate.Gender = candidate.Candidate.Gender;

                //Thông tin Job
                Model.JobApplication.Id = candidateId;
                Model.JobApplication.CreationTime = candidate.JobApplication.CreationTime;
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

                Model.JobApplication.WorkExperiences = new List<WorkExperienceEditDto>();  // Khởi tạo danh sách mới

                if (candidate.JobApplication.WorkExperiences != null)
                {
                    Model.JobApplication.WorkExperiences.AddRange(candidate.JobApplication.WorkExperiences);
                }
                Model.JobApplication.LearningProcess = new List<LearningProcessEditDto>();  // Khởi tạo danh sách mới

                if (candidate.JobApplication.LearningProcess != null)
                {
                    Model.JobApplication.LearningProcess.AddRange(candidate.JobApplication.LearningProcess);
                }
                await SetUserImageSourceAsync(Model);

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }


         
        }
        private async Task SetUserImageSourceAsync(NguoiTimViecModal nguoiTimViec)
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