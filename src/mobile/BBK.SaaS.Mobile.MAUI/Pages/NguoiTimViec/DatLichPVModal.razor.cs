using Abp.Application.Services.Dto;
using Abp.ObjectMapping;
using Abp.Runtime.Validation.Interception;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.GeoUnit;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using BBK.SaaS.ProfileNTD;
using BBK.SaaS.ViecTimNguoi;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class DatLichPVModal : ModalBase
    {
        public override string ModalId => "Book-Job";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
        protected IApplicationContext ApplicationContext { get; set; }
        protected IGeoUnitAppService GeoUnitAppService;
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IRecruiterAppService recruiterAppService { get; set; }
        private ItemsProviderResult<RecruitmentDto> recruitmenDto;

        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        private readonly CatFilterList _filterCat = new CatFilterList();
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }

        private Virtualize<RecruitmentDto> RecruitmentContainer { get; set; }

        protected string TenancyName { get; set; }
        public string RanksName { get; set; }
        public long RankId { get; set; }
        public DatLichPVModal()
        {
            makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            GeoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            recruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();

        }
        private List<RecruitmentDto> _applicationRequest { get; set; }

        public List<RecruitmentDto> ApplicationRequest
        {
            get => _applicationRequest;
            set => _applicationRequest = value;
        }
        protected DatLichModel Model = new();
        // protected NTDDatLichModel Model = new ();
        private bool _isInitialized;

        private List<CatUnitDto> _rank { get; set; }

        public List<CatUnitDto> Rank
        {
            get => _rank;
            set => _rank = value;
        }

        private string RankOfRecruitmenDto { get; set; }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {
                 _rank = catUnit.Rank;

                 var filterList = new CatFilterList()
                 {
                     Rank = _rank

                 };

                 CatFilterListDto = new ItemsProviderResult<CatFilterList>(
                     items: new List<CatFilterList> { filterList },
                     totalItemCount: 1
                 );
             });

            await UserDialogsService.UnBlock();
            return CatFilterListDto;


        }


        public async Task OpenFor(NguoiTimViecModal nTDDatLichModel)
        {
            _isInitialized = false;
            try
            {
                await SetBusyAsync(async () =>
                {
                    var geoUnit = await recruitmentAppService.GetAllBy();
                    if (geoUnit != null)
                    {
                        ApplicationRequest = geoUnit.Items.ToList();
                    }
                    Model = new DatLichModel();
                    Model.InterviewTime = DateTime.Now;
                    Model.CandidateId = nTDDatLichModel.Candidate.Id.Value;
                    Model.StatusOfCandidate = 1;
                    Model.JobApplicationId = nTDDatLichModel.JobApplication.Id.Value;
                    Model.TenantId = 1;
                    Model.TypeInterview = "1";
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                            _isInitialized = true;

                        }
                    );
                });
                await Show();
                await LoadFilter(new ItemsProviderRequest());
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }

        public async void selectedValue(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            Model.ApplicationRequestId = select;

            var rank = await recruitmentAppService.GetDetail(select);
            RanksName = rank.Ranks.DisplayName;
            RankId = rank.Rank;
            StateHasChanged();
        }
        public async void selectInterview(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            Model.TypeInterview = select;
            StateHasChanged();
        }
        private async Task<bool> ValidateInput()
        {

            if (Model.ApplicationRequestId == null || Model.ApplicationRequestId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn công việc mời phỏng vấn!"));
                return false;
            }
            //if (Model.TypeInterview == null || (string.IsNullOrEmpty(Model.TypeInterview)))
            //{
            //    await UserDialogsService.AlertWarn(@L("Vui lòng nhập hình thức phỏng vấn!"));
            //    return false;
            //}
            if (Model.Address == null || (string.IsNullOrEmpty(Model.Address)))
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng nhập địa chỉ phỏng vấn!"));
                return false;
            }
            if (Model.InterviewTime <= DateTime.Now)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn thời gian phỏng vấn hợp lý!"));
                return false;

            }
            return true;

        }
        private async Task SaveBookJob()
        {
            try
            {
                var candidate = await recruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });
                if (candidate != null)
                {
                    Model.RecruiterId = candidate.Recruiter.Id.Value;
                }
                Model.Rank = RankId;
                //Model.Ranks.DisplayName = RanksName;

                if (!await ValidateInput())
                {
                    return;
                }
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () => await makeAnAppointmentAppService.Create(Model),
                    async () =>
                    {
                        await UserDialogsService.AlertSuccess(L("Đặt lịch thành công"));
                        await Hide();
                        await OnSave.InvokeAsync();
                    }
                    );
                });
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }

    }
}
