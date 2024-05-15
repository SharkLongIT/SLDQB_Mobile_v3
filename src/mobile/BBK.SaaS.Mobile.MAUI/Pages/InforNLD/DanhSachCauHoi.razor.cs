using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class DanhSachCauHoi : SaaSMainLayoutPageComponentBase
    {
        protected IContactAppService contactAppService;
        protected INavigationService navigationService { get; set; }
        private Virtualize<ContactDto> contactContainer { get; set; }
        private readonly ContactSearch _filter = new ContactSearch();
        private ItemsProviderResult<ContactDto> contactDto;


        private string _SearchText = "";
        private long _Status = 0;
        public DanhSachCauHoi()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            contactAppService = DependencyResolver.Resolve<IContactAppService>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Danh sách câu hỏi của tôi"));
        }

        public async void selectStatus(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            _Status = select;
            StateHasChanged();
        }
        private bool _IsCancelList;
        private async Task RefeshList()
        {
            _IsCancelList = true;
            _SearchText = _filter.Search;
            if (_Status == 0)
            {
                _filter.Status = null;
            }
            else if (_Status == 1)
            {
                _filter.Status = true;
            }
            else
            {
                _filter.Status = false;
            }
            await contactContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadContact(new ItemsProviderRequest());
        }
        private async Task CancelList()
        {
            _SearchText = "";
            _Status = 0;
            _IsCancelList = false;
            await contactContainer.RefreshDataAsync();
            StateHasChanged();
            await LoadContact(new ItemsProviderRequest());
        }
        private async ValueTask<ItemsProviderResult<ContactDto>> LoadContact(ItemsProviderRequest request)
        {

            _filter.MaxResultCount = Math.Clamp(request.Count, 1, 1000);
            _filter.SkipCount = request.StartIndex;
            //_filter.Take = Math.Clamp(request.Count, 1, 1000);
            _filter.Search = _SearchText;
            if (_Status == 0)
            {
                _filter.Status = null;
            }
            else if (_Status == 1)
            {
                _filter.Status = true;
            }
            else
            {
                _filter.Status = false;
            }

            await UserDialogsService.Block();
            try
            {
                await WebRequestExecuter.Execute(
                              async () => await contactAppService.GetAllOfMe(_filter),
                              async (result) =>
                              {
                                  //var makeAnAppointment = result.Items.ToList();
                                  var contacts = ObjectMapper.Map<List<ContactDto>>(result.Items);
                                  //if (makeAnAppointment.Count == 0)
                                  //{
                                  //    isError = true;
                                  //}
                                  //else
                                  //{
                                  //    isError = false;
                                  //}
                                  contactDto = new ItemsProviderResult<ContactDto>(contacts, contacts.Count);
                                  await UserDialogsService.UnBlock();
                              }
                          );

                return contactDto;
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
        private ViewContactModal contactModal { get; set; }
        public async Task ViewContact(ContactDto contact)
        {
            await contactModal.OpenFor(contact);
        }
        private async void DisPlayAction(ContactDto contactDto)
        {
            string response = await App.Current.MainPage.DisplayActionSheet("Lựa chọn", null, null, "Xem chi tiết");
            if (response == "Xem chi tiết")
            {
                await ViewContact(contactDto);
            }
        }
    }
}
