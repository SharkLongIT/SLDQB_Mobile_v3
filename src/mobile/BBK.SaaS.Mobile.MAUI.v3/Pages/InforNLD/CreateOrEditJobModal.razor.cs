using Abp.Application.Services.Dto;
using Abp.Localization;
using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Security;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Services.Permission;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class CreateOrEditJobModal : ModalBase
    {
        public override string ModalId => "create-or-edit-job";

        [Parameter] public EventCallback OnSave { get; set; }
        protected IJobApplicationAppService JobApplicationAppService;
        protected ICandidateAppService CandidateAppService;
        protected IApplicationContext ApplicationContext;
        protected ICatUnitAppService CatUnitAppService;
        protected IGeoUnitAppService GeoUnitAppService;
        protected IPermissionService PermissionService;
        protected INavigationService navigationService { get; set; }

        protected NguoiTimViecModal UserInput;
        protected CreateOrEditJobModel Model;

        private List<CatUnitDto> _degree { get; set; }
        private List<CatUnitDto> _career { get; set; }
        private List<CatUnitDto> _rank { get; set; }
        private List<CatUnitDto> _formOfWork { get; set; }
        private List<CatUnitDto> _experience { get; set; }
        private List<CatUnitDto> _salary { get; set; }
        private List<CatUnitDto> _staffSize { get; set; }
        private List<GeoUnitDto> _workSite { get; set; }
        private bool _isInitialized;
        private bool IsEdit;

        private long IdCatUnitParent { get; set; }

        public CreateOrEditJobModal()
        {
            JobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            CandidateAppService = DependencyResolver.Resolve<ICandidateAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            GeoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
        }



        public List<CatUnitDto> Degree
        {
            get => _degree;
            set => _degree = value;
        }

        public List<CatUnitDto> Career
        {
            get => _career;
            set => _career = value;
        }

        public List<CatUnitDto> Rank
        {
            get => _rank;
            set => _rank = value;
        }

        public List<CatUnitDto> FormOfWork
        {
            get => _formOfWork; set => _formOfWork = value;
        }

        public List<CatUnitDto> Experience
        {
            get => _experience;
            set => _experience = value;
        }

        public List<CatUnitDto> Salary
        {
            get => _salary;
            set => _salary = value;
        }

        public List<CatUnitDto> StaffSize
        {
            get => _staffSize;
            set => _staffSize = value;
        }
        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
        }

        public async Task OpenFor(CreateOrEditJobModel jobApplication)
        {

            _isInitialized = false;
            IsEdit = false;
            try
            {


                await SetBusyAsync(async () =>
                {
                    var geoUnit = await GeoUnitAppService.GetGeoUnits();
                    if (geoUnit != null)
                    {
                        WorkSite = geoUnit.Items.Where(x => x.ParentId == null).ToList();
                    }

                    Model = new CreateOrEditJobModel();
                    Model.PositionsId = 0;
                    await WebRequestExecuter.Execute(
                        async () => await CatUnitAppService.GetFilterList(),
                        async (catUnit) =>
                        {
                            _degree = catUnit.Degree;
                            _career = catUnit.Career;
                            _experience = catUnit.Experience;
                            _formOfWork = catUnit.FormOfWork;
                            _rank = catUnit.Rank;
                            _isInitialized = true;
                            IsEdit = false;
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

        public async Task OpenForEdit(CreateOrEditJobModel jobApplication)
        {

            _isInitialized = false;
            IsEdit = true;
            try
            {

                var jobApplicationForEdit = await JobApplicationAppService.GetJobApplicationForEdit(new NullableIdDto<long> { Id = jobApplication.Id });
                jobApplication = ObjectMapper.Map<CreateOrEditJobModel>(jobApplicationForEdit.JobApplication);
                await SetBusyAsync(async () =>
                {
                    var geoUnit = await GeoUnitAppService.GetGeoUnits();
                    if (geoUnit != null)
                    {
                        WorkSite = geoUnit.Items.Where(x => x.ParentId == null).ToList();
                    }

                    Model = jobApplication;
                    await WebRequestExecuter.Execute(
                        async () => await CatUnitAppService.GetFilterList(),
                        async (catUnit) =>
                        {
                            _degree = catUnit.Degree;
                            _career = catUnit.Career;
                            _experience = catUnit.Experience;
                            _formOfWork = catUnit.FormOfWork;
                            _rank = catUnit.Rank;
                            _isInitialized = true;
                            IsEdit = true;
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
            if (string.IsNullOrEmpty(Model.Title))
            {
                await UserDialogsService.AlertWarn(@L("Tiêu đề hồ sơ không được để trống"));
                return false;
            }
            if (Model.PositionsId == null || Model.PositionsId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Cấp bậc không được để trống"));
                return false;
            }
            if (Model.OccupationId == null || Model.OccupationId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Nghề nghiệp không được bỏ trống"));
                return false;
            }
            if (Model.LiteracyId == null || Model.LiteracyId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Bằng cấp không được để trống"));
                return false;
            }

            if (Model.ExperiencesId == null || Model.ExperiencesId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Số năm kinh nghiệm không được để trống"));
                return false;
            }
            if (Model.FormOfWorkId == null || Model.FormOfWorkId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Hình thức làm việc không được để trống"));
                return false;
            }
            if (Model.WorkSite == null || Model.WorkSite <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Nơi muốn làm việc không được để trống"));
                return false;
            }

            if (string.IsNullOrEmpty(Model.Career))
            {
                await UserDialogsService.AlertWarn(@L("Mục tiêu không được để trống"));
                return false;
            }
            return true;

        }

        private async Task SaveJob()
        {
            await SetBusyAsync(async () =>
            {
                var candidate = await CandidateAppService.GetCandidateForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });
                if (candidate != null)
                {
                    Model.CandidateId = candidate.Candidate.Id.Value;
                }

                if (!await ValidateInput())
                {
                    return;
                }

                if (IsEdit != true)
                {
                    await WebRequestExecuter.Execute(
                  async () => await JobApplicationAppService.CreateJobApplication(Model),
                  async (result) =>
                  {
                      await UserDialogsService.AlertSuccess(L("Thêm mới thành công"));
                      await Hide();
                      await OnSave.InvokeAsync();
                      Model.Id = result.Id;
                  }
               );
                }
                else
                {
                    await WebRequestExecuter.Execute(
                  async () => await JobApplicationAppService.UpdateJobApplicationForWeb(new JobApplicationCreate()
                  {
                      Id = Model.Id,
                      TenantId = Model.TenantId,
                      DesiredSalary = Model.DesiredSalary,
                      FormOfWorkId = Model.FormOfWorkId,
                      Career = Model.Career,
                      LiteracyId = Model.LiteracyId,
                      PositionsId = Model.PositionsId.Value,
                      OccupationId = Model.OccupationId,
                      WorkSite = Model.WorkSite,
                      ExperiencesId = Model.ExperiencesId,
                      Title = Model.Title,
                      IsPublished = Model.IsPublished,
                      CandidateId = Model.CandidateId,
                      FileCVUrl = StringCipher.Instance.Encrypt(Model.FileCVUrl),


                  }),
                      async (result) =>
                      {
                          await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                          await Hide();
                          await OnSave.InvokeAsync();
                          Model.Id = result.Id;
                      }
                   );
                }

                //navigationService.NavigateTo($"ChiTietBUT?Id={Model.Id}");
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
