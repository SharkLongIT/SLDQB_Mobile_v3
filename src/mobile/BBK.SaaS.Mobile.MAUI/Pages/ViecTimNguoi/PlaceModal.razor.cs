using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class PlaceModal : ModalBase
    {
        public override string ModalId => "place-modal";

        
        [Parameter] public EventCallback<string> OnChange { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IGeoUnitAppService geoUnitAppService { get; set; }
        protected NavigationManager navigationManager { get; set; }
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }
        private readonly RecruimentInput _filter = new RecruimentInput();

        private List<GeoUnitDto> _workSite { get; set; }

        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
        }
        public PlaceModal() 
        {
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();

        }
        #region GeoUnit

        private async ValueTask<ItemsProviderResult<GeoUnitDto>> LoadGeoUnit(ItemsProviderRequest request)
        {

            await WebRequestExecuter.Execute(
                async () => await geoUnitAppService.GetGeoUnits(),
                async (result) =>
                {
                    var geoUnit = result.Items.Where(x => x.ParentId == null).ToList();
                    int totalItems = 1;

                    geoUnitDto = new ItemsProviderResult<GeoUnitDto>(geoUnit, totalItems);
                    if (geoUnitDto.Items != null)
                    {
                        WorkSite = geoUnit;
                    }
                });

            await UserDialogsService.UnBlock();

            return geoUnitDto;
        }


        #endregion

        public async Task OpenFor()
        {
            try
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        //async () => await jobApplicationAppService.GetListJobAppOfCandidate(new JobAppSearch()),
                        async () =>
                        {
                            //_jobApplicationEditDto = result.Items.ToList();
                            //_isInitialized = true;
                            //Model.RecruitmentId = ViecTimNguoiModel.Id.Value;
                            //Model.Status = 1;
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

        public long WorkSiteId { get; set; }
        public string WorkSiteName { get; set; }
        public async void selectedValue(ChangeEventArgs args)
        {
            try
            {
                if (args.Value != null)
                {
                    long select = Convert.ToInt64(args.Value);
                    WorkSiteId = select;
                    var selectedWorkSite = WorkSite.FirstOrDefault(item => item.Id == WorkSiteId);
                    if (selectedWorkSite != null)
                    {
                        WorkSiteName = selectedWorkSite.DisplayName;
                    }
                    else
                    {
                        WorkSiteName = string.Empty;
                    }
                }
                await OnChange.InvokeAsync();
                await Hide();   

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }
    }
}
