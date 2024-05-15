using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Services.User;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class DanhSachLichHen : SaaSMainLayoutPageComponentBase
    {
        protected INavigationService navigationService { get; set; }
        private string _SearchText = "";
        private bool isError = false;
        private bool IsUserLoggedIn;
        protected IApplicationContext ApplicationContext { get; set; }

        protected IMakeAnAppointmentAppService makeAnAppointmentAppService;
        private Virtualize<MakeAnAppointmentDto> makeAnAppointmentContainer { get; set; }
        private readonly MakeAnAppointmentInput _filter = new MakeAnAppointmentInput();
        private ItemsProviderResult<MakeAnAppointmentDto> makeAnAppointmentDto;
        protected IUserProfileService UserProfileService { get; set; }
        protected IAccessTokenManager AccessTokenManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            IsUserLoggedIn = navigationService.IsUserLoggedIn();
            await LoadListMakeAnAppointment(new ItemsProviderRequest());
        }
        public DanhSachLichHen()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            makeAnAppointmentAppService = DependencyResolver.Resolve<IMakeAnAppointmentAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            UserProfileService = DependencyResolver.Resolve<IUserProfileService>();

        }
        private async Task RefeshList()
        {
            _SearchText = _filter.Search;
            await makeAnAppointmentContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadListMakeAnAppointment(new ItemsProviderRequest());
        }
        public async void selectedValue(ChangeEventArgs args)
        {
            string select = Convert.ToString(args.Value);
            _SearchText = select;
            await makeAnAppointmentContainer.RefreshDataAsync();
            StateHasChanged();

        }
        private async ValueTask<ItemsProviderResult<MakeAnAppointmentDto>> LoadListMakeAnAppointment(ItemsProviderRequest request)
        {

            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = _SearchText;
         

            await UserDialogsService.Block();
            try
            {
                await WebRequestExecuter.Execute(
                              async () => await makeAnAppointmentAppService.GetAll(_filter),
                              async (result) =>
                              {
                                  var makeAnAppointment = result.Items.ToList();
                                  if (makeAnAppointment.Count == 0)
                                  {
                                      isError = true;
                                  }
                                  else
                                  {
                                      isError = false;
                                  }
                                  makeAnAppointmentDto = new ItemsProviderResult<MakeAnAppointmentDto>(makeAnAppointment, makeAnAppointment.Count);
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
        private ChiTietLichHen chiTietLichHen { get; set; }
        public async Task ViewAppointment(MakeAnAppointmentDto appointment)
        {
            //navigationService.NavigateTo($"ChiTietLichHen?Id={appointment.Id}&Name={appointment.Candidate.Account.Name}&Rank={appointment.Ranks.DisplayName}&Title={appointment.JobApplication.Title}");
            await chiTietLichHen.OpenFor(appointment);
        }

        private UpdateAppointmentModal updateAppointmentModal {  get; set; }
        public async Task UpdatePost(MakeAnAppointmentDto appointmentDto)
        {
            await updateAppointmentModal.OpenFor(appointmentDto);
        }
        private async void DisPlayAction(MakeAnAppointmentDto appointmentDto)
        {
            if (appointmentDto.StatusOfCandidate == 1)
            {
                string view = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Xem", null);
                if (view == "Xem")
                {
                    await ViewAppointment(appointmentDto);
                }
            }
            else
            {
                string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Xem", "Cập nhật");
                if (response == "Xem")
                {
                    await ViewAppointment(appointmentDto);
                }
                else if (response == "Cập nhật")
                {
                    await UpdatePost(appointmentDto);

                }
            }
          
        }
    }
}
