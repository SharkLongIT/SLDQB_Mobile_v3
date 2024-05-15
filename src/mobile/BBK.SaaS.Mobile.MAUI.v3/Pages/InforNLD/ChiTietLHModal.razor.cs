using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class ChiTietLHModal : ModalBase
    {
        public override string ModalId => "Detail-Job";
        public bool IsView = false;
        public bool IsAgree = false;
        public bool IsReject = false;
        public string InterviewTime;

        [Parameter] public EventCallback<string> OnSave { get; set; }
        protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
        protected IApplicationContext ApplicationContext { get; set; }

        public ChiTietLHModal()
        {
            makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        protected virtual async Task Cancel()
        {
            await Hide();
        }

        protected DatLichModel UserInput;
        protected NTDDatLichModel Model = new();
        private bool _isInitialized;


        //
        public async Task OpenFor(DatLichModel DatLichModel)
        {
            IsReject = DatLichModel.IsReject;
            IsAgree = DatLichModel.IsAgree;
            IsView = DatLichModel.IsView;
            _isInitialized = false;
            try
            {
                var datlich = await makeAnAppointmentAppService.GetDetail(DatLichModel.Id.Value);
                if (IsView == true)
                {
                    await SetBusyAsync(async () =>
                    {
                        UserInput = await ConvertMakeAnAppointmentDtoToDatLichModel(datlich);
                        IsView = true;
                        IsAgree = false;
                        IsReject = false;
                        InterviewTime = UserInput.InterviewTime.ToLongTimeString();
                        _isInitialized = true;
                    });
                }
                if (IsAgree == true)
                {
                    await SetBusyAsync(async () =>
                    {
                        UserInput = await ConvertMakeAnAppointmentDtoToDatLichModel(datlich);
                        IsView = false;
                        IsAgree = true;
                        IsReject = false;
                        InterviewTime = UserInput.InterviewTime.ToLongTimeString();
                        _isInitialized = true;
                    });
                }
                if (IsReject == true)
                {
                    await SetBusyAsync(async () =>
                    {
                        UserInput = await ConvertMakeAnAppointmentDtoToDatLichModel(datlich);
                        IsView = false;
                        IsAgree = false;
                        IsReject = true;
                        InterviewTime = UserInput.InterviewTime.ToLongTimeString();
                        _isInitialized = true;
                    });
                }

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

            await Show();
        }

        private async Task SaveExpWork()
        {
            await SetBusyAsync(async () =>
            {
                if (IsAgree == true)
                {
                    UserInput.StatusOfCandidate = 2;
                }
                if (IsReject == true)
                {
                    UserInput.StatusOfCandidate = 3;
                }
                await WebRequestExecuter.Execute(
                async () => await makeAnAppointmentAppService.UpdateForCandidate(UserInput),
                async (result) =>
                {
                    if (result.StatusOfCandidate == 2)
                    {
                        await UserDialogsService.AlertSuccess(L("Xác nhận thành công"));
                        StateHasChanged();
                    }
                    else if (result.StatusOfCandidate == 3)
                    {
                        await UserDialogsService.AlertSuccess(L("Từ chối thành công"));
                        StateHasChanged();

                    }

                    await Hide();
                    await OnSave.InvokeAsync();

                }
               );

            });

        }

        private async Task<DatLichModel> ConvertMakeAnAppointmentDtoToDatLichModel(MakeAnAppointmentDto MakeAnAppointmentDto)
        {
            DatLichModel DatLichModel = new DatLichModel();

            DatLichModel.Id = MakeAnAppointmentDto.Id;

            DatLichModel.TenantId = MakeAnAppointmentDto.TenantId;

            DatLichModel.TypeInterview = MakeAnAppointmentDto.TypeInterview;

            DatLichModel.Address = MakeAnAppointmentDto.Address;

            DatLichModel.Message = MakeAnAppointmentDto.Message;

            DatLichModel.InterviewTime = MakeAnAppointmentDto.InterviewTime;
            DatLichModel.CandidateId = MakeAnAppointmentDto.CandidateId;
            DatLichModel.JobApplicationId = MakeAnAppointmentDto.JobApplicationId;
            DatLichModel.RecruiterId = MakeAnAppointmentDto.RecruiterId;
            DatLichModel.ApplicationRequestId = MakeAnAppointmentDto.ApplicationRequestId;
            DatLichModel.StatusOfCandidate = MakeAnAppointmentDto.StatusOfCandidate;
            DatLichModel.Rank = MakeAnAppointmentDto.Rank;
            DatLichModel.InterviewResultStatus = MakeAnAppointmentDto.InterviewResultStatus;
            DatLichModel.InterviewResultLetter = MakeAnAppointmentDto.InterviewResultLetter;
            DatLichModel.Recruiter = MakeAnAppointmentDto.Recruiter;
            DatLichModel.JobApplication = MakeAnAppointmentDto.JobApplication;
            DatLichModel.Candidate = MakeAnAppointmentDto.Candidate;
            DatLichModel.Ranks = MakeAnAppointmentDto.Ranks;
            DatLichModel.Recruitment = MakeAnAppointmentDto.Recruitment;
            DatLichModel.Name = MakeAnAppointmentDto.Name;
            DatLichModel.ReasonForRefusal = MakeAnAppointmentDto.ReasonForRefusal;
            return DatLichModel;
        }

    }
}
