using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class UpdateAppointmentModal : ModalBase
    {
        public override string ModalId => "update-appointment";
        protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
        protected IRecruiterAppService recruiterAppService;
        public IApplicationContext ApplicationContext { get; set; }

        string ApplicationRequest;
        string RanksName;
        [Parameter] public EventCallback OnSave { get; set; }
        public UpdateAppointmentModal() {

            makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
            recruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        public MakeAnAppointmentDto Model = new MakeAnAppointmentDto();
        public async Task OpenFor(MakeAnAppointmentDto appointmentDto)
        {
            var applicationRequest = await makeAnAppointmentAppService.GetDetail(appointmentDto.Id.Value);
            try
            {
                await SetBusyAsync(async () =>
                {
                     //Model.Name = appointmentDto.Name;
                     ApplicationRequest = applicationRequest.Recruitment.Title; // Cv moi pv
                     Model.Id= applicationRequest.Id.Value;
                     Model.ApplicationRequestId = applicationRequest.Recruitment.Id.Value; // id cv
                     Model.Address = appointmentDto.Address; 
                     Model.Candidate = appointmentDto.Candidate;
                     Model.CandidateId = appointmentDto.CandidateId; 
                     Model.Rank = appointmentDto.Rank; // vi tri
                     Model.Address = applicationRequest.Address;
                     Model.Ranks = appointmentDto.Ranks;
                     Model.Recruiter = appointmentDto.Recruiter;
                     Model.Recruitment = appointmentDto.Recruitment;
                     Model.TypeInterview = appointmentDto.TypeInterview;
                     RanksName = appointmentDto.Ranks.DisplayName; // vi tri
                     Model.JobApplication = appointmentDto.JobApplication;
                     Model.JobApplicationId = appointmentDto.JobApplicationId;
                     Model.Message = applicationRequest.Message;
                     Model.InterviewTime = appointmentDto.InterviewTime;
                     Model.ReasonForRefusal = applicationRequest.ReasonForRefusal;
                     Model.InterviewResultLetter = applicationRequest.InterviewResultLetter;
                     Model.StatusOfCandidate = applicationRequest.StatusOfCandidate;
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
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
        private async Task SaveJob()
        {
            await SetBusyAsync(async () =>
            {
                var recruiterId = await recruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });
                if (recruiterId != null)
                {
                    Model.RecruiterId = recruiterId.Recruiter.Id.Value;
                }
                Model.TenantId = ApplicationContext.CurrentTenant.TenantId;
                await WebRequestExecuter.Execute(
                  async () => await makeAnAppointmentAppService.UpdateAppForMobile(new MakeAnAppointmentForUpdateMobile()
                  {
                      Id = Model.Id,
                      JobApplicationId = Model.JobApplicationId,
                      InterviewResultLetter = Model.InterviewResultLetter,
                  }),
                  async (result) =>
                  {
                      await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                      await Hide();
                      await OnSave.InvokeAsync();
                  }
               );
            });


        }
    }
}
