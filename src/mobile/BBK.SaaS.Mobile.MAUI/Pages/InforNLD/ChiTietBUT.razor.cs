using Abp.Application.Services.Dto;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Models.InforNLD;
using BBK.SaaS.Mobile.MAUI.Services.UI;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class ChiTietBUT : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IUserProfileService UserProfileService { get; set; }
        protected UserDialogsService UserDialogsService { get; set; }

        protected IApplicationContext ApplicationContext { get; set; }
        private bool IsUserLoggedIn;
        private string _userImage;


        [Parameter]
        public int Id { get; set; } = 0;
        //private ItemsProviderResult<GetJobApplicationForEditOutput> jobApplication;

        protected NguoiTimViecModal Model = new();



        public ChiTietBUT()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();

            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserDialogsService = DependencyResolver.Resolve<UserDialogsService>();

        }
        protected override async Task OnInitializedAsync()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();

            await SetPageHeader(L("Thông tin bài ứng tuyển"), new List<Services.UI.PageHeaderButton>());

            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ChiTietBUT") + "ChiTietBUT".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            int? candidateId = null;

            if (q1["Id"] != null)
            {
                candidateId = int.Parse(q1["Id"]);
            }

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
            Model.Candidate.AvatarUrl = candidate.Candidate.AvatarUrl;
            Model.Candidate.DateOfBirth = candidate.Candidate.DateOfBirth;
            //Model.Candidate.PhoneNumber = candidate.Candidate.PhoneNumber;
            Model.Candidate.Marital = candidate.Candidate.Marital;
            Model.Candidate.Gender = candidate.Candidate.Gender;

            //Thông tin Job
            Model.JobApplication.Id = candidateId;
            Model.JobApplication.CreationTime = candidate.JobApplication.CreationTime;
            Model.JobApplication.LastModificationTime = candidate.JobApplication.LastModificationTime;
            Model.JobApplication.Career = candidate.JobApplication.Career;
            Model.Positions = candidate.JobApplication.Positions.DisplayName;
            Model.JobApplication.CurrencyUnit = candidate.JobApplication.CurrencyUnit;
            Model.JobApplication.DesiredSalary = candidate.JobApplication.DesiredSalary;
            Model.Literacy = candidate.JobApplication.Literacy.DisplayName;
            //Model.JobApplication.WorkExperienceDtos = candidate.JobApplication.WorkExperienceDtos;
            Model.FormOfWork = candidate.JobApplication.FormOfWork.DisplayName;
            //Model.JobApplication.LearningProcessDtos = candidate.JobApplication.LearningProcessDtos;
            Model.JobApplication.Word = candidate.JobApplication.Word;
            Model.JobApplication.PowerPoint = candidate.JobApplication.PowerPoint;
            Model.JobApplication.Excel = candidate.JobApplication.Excel;
            Model.JobApplication.WorkExperiences = new List<WorkExperienceEditDto>();  

            if (candidate.JobApplication.WorkExperiences != null)
            {
                Model.JobApplication.WorkExperiences.AddRange(candidate.JobApplication.WorkExperiences);
            }
            Model.JobApplication.LearningProcess = new List<LearningProcessEditDto>(); 

            if (candidate.JobApplication.LearningProcess != null)
            {
                Model.JobApplication.LearningProcess.AddRange(candidate.JobApplication.LearningProcess);
            }
            await GetUserPhoto();

        }

        private async Task RefreshList()
        {
            await OnInitializedAsync();
            StateHasChanged();
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


        #region Create/Edit ExpWork
        private CreateExpWorkModal createExpWorkModal { get; set; }
        public async Task EditUser(WorkExperienceEditDto workExperienceEditDto)
        {
            await createExpWorkModal.OpenForEdit(workExperienceEditDto);
        }
        public async Task OpenCreateModal()
        {
            await createExpWorkModal.OpenFor(Model);
        }
        #endregion

        #region Create/Edit learning
        private CreateLearningProcessModal createLearningProcessModal { get; set; }
        public async Task EditLearning(LearningProcessEditDto learningProcessEditDto)
        {
            await createLearningProcessModal.OpenForEdit(learningProcessEditDto);
        }
        public async Task OpenCreateLearningProcessModal()
        {
            await createLearningProcessModal.OpenFor(Model);
        }
        #endregion

        private async void DisPlayAction(WorkExperienceEditDto workExperienceEditDto)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Chi tiết kinh nghiệm làm việc", null, null, "Xóa", "Sửa kinh nghiệm làm việc");
            if (response == "Sửa kinh nghiệm làm việc")
            {
                await EditUser(workExperienceEditDto);
            }
            else if (response == "Xóa")
            {
                var Isdelete = await UserDialogsService.Confirm("Bạn có chắc chắn xóa kinh nghiệm làm việc không?", "Xóa kinh nghiệm làm việc");

                if (Isdelete == true)
                {
                    await jobApplicationAppService.DeleteWorkExperience(workExperienceEditDto);
                    await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                    await RefreshList();
                }
                else
                {
                    DisPlayAction(workExperienceEditDto);
                }
            }

          
        } 
        private async void DisPlayActionLearn(LearningProcessEditDto learningProcessEditDto)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Chi tiết trình độ học vấn", null, null, "Xóa", "Sửa trình độ học vấn");
            if (response == "Sửa trình độ học vấn")
            {
                await EditLearning(learningProcessEditDto);
            }
            else if (response == "Xóa")
            {
                var Isdelete = await UserDialogsService.Confirm("Bạn có chắc chắn xóa trình độ học vấn không?", "Xóa trình độ học vấn");

                if (Isdelete == true)
                {
                    await jobApplicationAppService.DeleteLearningProcess(learningProcessEditDto);
                    await UserDialogsService.AlertSuccess(L("Xóa thành công"));
                    await RefreshList();
                }
                else
                {
                    DisPlayActionLearn(learningProcessEditDto);
                }
            }
        }
    }
}
