using Abp.UI;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using Abp.Extensions;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class DanhSachLH : SaaSMainLayoutPageComponentBase
    {
        protected INavigationService navigationService { get; set; }
        private string _SearchText = "";
        private bool isError = false;

        private string ReasonForRefusal;

        protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
        private Virtualize<DatLichModel> makeAnAppointmentContainer { get; set; }
        private readonly MakeAnAppointmentInput _filter = new MakeAnAppointmentInput();
        private ItemsProviderResult<DatLichModel> makeAnAppointmentDto;

        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Danh sách lịch hẹn"));
        }
        public DanhSachLH()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
        }
        private bool _IsCancelList;

        private async Task RefeshList()
        {
            _IsCancelList = true;

            _SearchText = _filter.Search;
            await makeAnAppointmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadListMakeAnAppointment(new ItemsProviderRequest());
        }
        private async Task CancelList()
        {
            _SearchText = "";
            _IsCancelList = false;
            await makeAnAppointmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadListMakeAnAppointment(new ItemsProviderRequest());
        }
        private async ValueTask<ItemsProviderResult<DatLichModel>> LoadListMakeAnAppointment(ItemsProviderRequest request)
        {

            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = _SearchText;


            await UserDialogsService.Block();
            try
            {
                await WebRequestExecuter.Execute(
                              async () => await makeAnAppointmentAppService.GetAllOfCandidate(_filter),
                              async (result) =>
                              {
                                  //var makeAnAppointment = result.Items.ToList();
                                  var makeAnAppointment = ObjectMapper.Map<List<DatLichModel>>(result.Items);
                                  if (makeAnAppointment.Count == 0)
                                  {
                                      isError = true;
                                  }
                                  else
                                  {
                                      isError = false;
                                  }
                                  makeAnAppointmentDto = new ItemsProviderResult<DatLichModel>(makeAnAppointment, makeAnAppointment.Count);
                                  await UserDialogsService.UnBlock();
                              }
                          );

                return makeAnAppointmentDto;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }
        private ChiTietLHModal chiTietLHModal { get; set; }
        public async Task DetailJob(DatLichModel datLichModel)
        {
            await chiTietLHModal.OpenFor(datLichModel);
        }
        public async Task ViewAppointment(DatLichModel appointment)
        {
            //navigationService.NavigateTo($"ChiTietLH?Id={appointment.Id}&Name={appointment.Name}&Rank={appointment.Ranks.DisplayName}&Title={appointment.JobApplication.Title}");
            appointment.IsAgree = false;
            appointment.IsView = true;
            appointment.IsReject = false;
            await chiTietLHModal.OpenFor(appointment);
        }
        public async Task ViewJob(DatLichModel appointment)
        {
            navigationService.NavigateTo($"ThongTinVTN?Id={appointment.Recruitment.Id}&WorkAddress={appointment.Recruitment.WorkAddress}&Experiences={appointment.JobApplication.Experiences.DisplayName}&SphereOfActivity={appointment.Recruiter.SphereOfActivity.DisplayName}&HumanResSizeCat={appointment.Recruiter.HumanResSizeCat.DisplayName}");

        }
        private async void UpdateStatus(DatLichModel datLichModel)
        {
            if (datLichModel.StatusOfCandidate == 2 || datLichModel.StatusOfCandidate == 3)
            {
                string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Xem chi tiết", "Xem công việc");
                if (response == "Xem chi tiết")
                {
                    datLichModel.IsView = true;
                    await chiTietLHModal.OpenFor(datLichModel);
                }
                else if (response == "Xem công việc")
                {
                    await ViewJob(datLichModel);
                }
            }
            else
            {
                string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Từ chối", "Xác nhận", "Xem chi tiết");
                if (response == "Từ chối")
                {
                    datLichModel.IsAgree = false;
                    datLichModel.IsView = false;
                    datLichModel.IsReject = true;
                    await chiTietLHModal.OpenFor(datLichModel);
                    //makeAnAppointmentAppService.UpdateForCandidate(new MakeAnAppointmentDto()
                    //{
                    //    Id = datLichModel.Id,
                    //    StatusOfCandidate = 3,
                    //    // ReasonForRefusal = ReasonForRefusal
                    //});
                    //await UserDialogsService.AlertSuccess(L("Từ chối thành công"));
                }
                else if (response == "Xác nhận")
                {
                    datLichModel.IsAgree = true;
                    datLichModel.IsView = false;
                    datLichModel.IsReject = false;
                    await chiTietLHModal.OpenFor(datLichModel);
                    //makeAnAppointmentAppService.UpdateForCandidate(new MakeAnAppointmentDto()
                    //{
                    //    Id = datLichModel.Id,
                    //    StatusOfCandidate = 2,
                    //    // ReasonForRefusal = ReasonForRefusal
                    //});
                    //await UserDialogsService.AlertSuccess(L("Xác nhận thành công"));
                }
                else if (response == "Xem chi tiết")
                {
                    datLichModel.IsAgree = false;
                    datLichModel.IsView = true;
                    datLichModel.IsReject = false;
                    await chiTietLHModal.OpenFor(datLichModel);
                }
               
            }

        }
    }
}
