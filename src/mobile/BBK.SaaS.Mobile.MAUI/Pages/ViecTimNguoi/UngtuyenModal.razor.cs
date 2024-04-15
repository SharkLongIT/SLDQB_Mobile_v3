using Abp.ObjectMapping;
using Abp.Runtime.Validation.Interception;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Models.Ungtuyen;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.ProfileNTD;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class UngtuyenModal : ModalBase
    {
        public override string ModalId => "ApplicaitonRequest-modal";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        protected IApplicationRequestAppService applicationRequestAppService { get; set; }
        protected IJobApplicationAppService jobApplicationAppService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }
        protected INavigationService navigationService { get; set; }

        public UngtuyenModal()
        {
            applicationRequestAppService = DependencyResolver.Resolve<IApplicationRequestAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();

        }
        //protected virtual async Task Save()
        //{
        //    await Hide();
        //    await OnSave.InvokeAsync(TenancyName);
        //    TenancyName = null;
        //}

        //protected virtual async Task Cancel()
        //{
        //    TenancyName = null;
        //    await Hide();
        //}

        protected UngTuyenModel Model = new();
        
        private bool _isInitialized;

        private List<JobApplicationEditDto> _jobApplicationEditDto;

        public List<JobApplicationEditDto> JobApplicationEditDto
        {
            get => _jobApplicationEditDto;
            set => _jobApplicationEditDto = value;  
        }

        

        //
        public async Task OpenFor(ViecTimNguoiModel ViecTimNguoiModel)
        {
            _isInitialized = false;
            try
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () => await jobApplicationAppService.GetListJobAppOfCandidate(new JobAppSearch()),
                        async (result) =>
                        {
                            _jobApplicationEditDto = result.Items.ToList(); 
                            _isInitialized = true;
                            Model.RecruitmentId = ViecTimNguoiModel.Id.Value;
                            Model.Status = 1;
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

        private async Task<bool> ValidateInput()
        {

            if (Model.JobApplicationId == null || Model.JobApplicationId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Hồ sơ ứng tuyển không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.Content))
            {
                await UserDialogsService.AlertWarn(@L("Nội dung không được để trống"));
                return false;
            }
            return true;

        }

        private async Task SaveBookJob()
        {
            try
            {
                if (!await ValidateInput())
                {
                    return;
                }

                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                    async () => await applicationRequestAppService.Create(Model),
                    async (result) =>
                    {
                        await UserDialogsService.AlertSuccess(L("Ứng tuyển thành công"));
                        Model.Id = result.Id;
                        await Hide();
                        await OnSave.InvokeAsync();
                    }
                    );
                });
                navigationService.NavigateTo($"ChiTietCVDUT?Id={Model.Id}");
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }

    }
}
