using Abp.Application.Services.Dto;
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
    public partial class CreateLearningProcessModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }

        protected IJobApplicationAppService JobApplicationAppService;

        protected LearningProcessModel Model = new();

        protected NguoiTimViecModal nguoiTimViecModal;

        private bool IsEdit;

        public CreateLearningProcessModal()
        {
            JobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
        }
        public override string ModalId => "create-learn";
        public async Task OpenFor(NguoiTimViecModal nguoiTimViecModal)
        {
            //_isInitialized = false;

            try
            {
                IsEdit = false;
                await SetBusyAsync(async () =>
                {
                    Model = new LearningProcessModel();

                    if (nguoiTimViecModal.JobApplication.Id.HasValue)
                    {
                        Model.JobApplicationId = nguoiTimViecModal.JobApplication.Id.Value;
                        Model.StartTime = DateTime.Now;
                        Model.EndTime = DateTime.Now;
                        IsEdit = false;
                    }
                });
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

            await Show();
        }
        public async Task OpenForEdit(LearningProcessEditDto LearningProcessEditDto)
        {
            //_isInitialized = false;
            IsEdit = true;
            try
            {

                await SetBusyAsync(async () =>
                {
                    Model = new LearningProcessModel();
                    Model.Id = LearningProcessEditDto.Id;   
                    Model.JobApplicationId = LearningProcessEditDto.JobApplicationId;
                    Model.StartTime = LearningProcessEditDto.StartTime;
                    Model.EndTime = LearningProcessEditDto.EndTime;
                    Model.Description = LearningProcessEditDto.Description;
                    Model.SchoolName = LearningProcessEditDto.SchoolName;
                    Model.AcademicDiscipline = LearningProcessEditDto.AcademicDiscipline;
                    Model.TenantId = LearningProcessEditDto.TenantId;
                    IsEdit = true;
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
            if (Model.StartTime > Model.EndTime)
            {
                await UserDialogsService.AlertWarn(@L("Thời gian bắt đầu không được lớn hơn thời điểm kết thúc"));
                return false;
            }
            if (Model.EndTime > DateTime.Now)
            {
                await UserDialogsService.AlertWarn(@L("Thời gian kết thúc không được lớn hơn thời điểm hiện tại"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.SchoolName))
            {
                await UserDialogsService.AlertWarn(@L("Tên trường học không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.AcademicDiscipline))
            {
                await UserDialogsService.AlertWarn(@L("Ngành học không được để trống"));
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
            await SetBusyAsync(async () =>
            {

                if (!await ValidateInput())
                {
                    return;
                }

                if (IsEdit == true)
                {
                    await WebRequestExecuter.Execute(
               async () => await JobApplicationAppService.UpdateLearningProcess(Model),
             async (result) =>
             {
                 await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                 await Hide();
                 await OnSave.InvokeAsync();
             }
            );
                }
                else
                {
                    await WebRequestExecuter.Execute(
                                   async () => await JobApplicationAppService.CreateLearningProcess(Model),
                                 async (result) =>
                                 {
                                     await UserDialogsService.AlertSuccess(L("Thêm mới thành công"));
                                     await Hide();
                                     await OnSave.InvokeAsync();
                                 }
                                );
                }

            });

        }
        private async Task CancelModal()
        {
            var IsCancel = await UserDialogsService.Confirm("Bạn có chắc chắn rời khỏi?", "");
            if (IsCancel == true)
            {
                await Hide();
            }
            else
            {

            }
        }
    }
}
