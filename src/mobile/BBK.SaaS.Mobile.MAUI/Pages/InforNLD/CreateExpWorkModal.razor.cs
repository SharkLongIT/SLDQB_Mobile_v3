using Abp.Application.Services.Dto;
using Abp.Runtime.Validation.Interception;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.InforNLD;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class CreateExpWorkModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }

        protected IJobApplicationAppService JobApplicationAppService;

        protected WorkExperienceModel Model = new();

        protected NguoiTimViecModal nguoiTimViecModal;

        private bool isEdit;

        public CreateExpWorkModal()
        {
            JobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
        }
        public override string ModalId => "create-exp";
        public async Task OpenFor(NguoiTimViecModal nguoiTimViecModal)
        {
            //_isInitialized = false;
           
            try
            {
                isEdit = false;
                await SetBusyAsync(async () =>
                {
                    Model = new WorkExperienceModel();

                    if (nguoiTimViecModal.JobApplication.Id.HasValue)
                    {
                        Model.JobApplicationId = nguoiTimViecModal.JobApplication.Id.Value;
                        Model.StartTime = DateTime.Now;
                        Model.EndTime = DateTime.Now;   
                    }
                    isEdit = false;
                });
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

            await Show();
        }

        public async Task OpenForEdit(WorkExperienceEditDto workExperienceEditDto)
        {
            //_isInitialized = false;
            isEdit = true;
            try
            {

                await SetBusyAsync(async () =>
                {
                    Model = new WorkExperienceModel();

                    Model.JobApplicationId = workExperienceEditDto.JobApplicationId;
                    Model.Id = workExperienceEditDto.Id;
                    Model.TenantId = workExperienceEditDto.TenantId;
                    Model.StartTime = workExperienceEditDto.StartTime;
                    Model.EndTime = workExperienceEditDto.EndTime;
                    Model.Positions = workExperienceEditDto.Positions;
                    Model.CompanyName = workExperienceEditDto.CompanyName;
                    Model.Description = workExperienceEditDto.Description;

                    isEdit = true;
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
            // Since DataAnnotationsValidator doesn't work for nested object validation.
            // We manually do validation for each nested object.
            //var userValidationResult = DataAnnotationsValidator.Validate(Model);

            //if (userValidationResult.IsValid)
            //{
            //    return true;
            //}

            //await UserDialogsService.AlertWarn(userValidationResult.ValidationErrors.ConsolidatedMessage);
            //return false;
            if (string.IsNullOrEmpty(Model.CompanyName))
            {
                await UserDialogsService.AlertWarn(@L("Tên công ty không được để trống"));
                return false;
            }

            if (string.IsNullOrEmpty(Model.Positions))
            {
                await UserDialogsService.AlertWarn(@L("Vị trí công việc không được để trống"));
                return false;
            }

            if (Model.StartTime.Date > Model.EndTime.Date)
            {
                await UserDialogsService.AlertWarn(@L("Thời gian bắt đầu không được lớn hơn thời điểm kết thúc"));
                return false;
            }
            if (Model.EndTime.Date > DateTime.Now.Date)
            {
                await UserDialogsService.AlertWarn(@L("Thời gian kết thúc không được lớn hơn thời điểm hiện tại"));
                return false;
            }
          
          
            if (string.IsNullOrEmpty(Model.Description))
            {
                await UserDialogsService.AlertWarn(@L("Mô tả không được để trống"));
                return false;
            }
            return true;

        }
        private async Task SaveExpWork()
        {

            if (!await ValidateInput())
            {
                return;
            }
            await SetBusyAsync(async () =>
            {
                if(isEdit == false)
                {
                    await WebRequestExecuter.Execute(
                                     async () => await JobApplicationAppService.CreateWorkExperience(Model),
                                   async (result) =>
                                   {
                                       await UserDialogsService.AlertSuccess(L("Thêm mới thành công"));
                                       await Hide();
                                       await OnSave.InvokeAsync();
                                   }
                                  );
                }
                else
                {
                    await WebRequestExecuter.Execute(
                                    async () => await JobApplicationAppService.UpdateWorkExperience(Model),
                                  async (result) =>
                                  {
                                      await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                                      await Hide();
                                      await OnSave.InvokeAsync();
                                  }
                                 );
                }
               
            });

        }
        private async Task CancelModal()
        {
            //var IsCancel = await UserDialogsService.Confirm("Bạn có chắc chắn rời khỏi?", "");
            //if (IsCancel == true)
            //{
            //    await Hide();
            //}
            //else
            //{

            //}
            await Hide();
        }
    }
}
