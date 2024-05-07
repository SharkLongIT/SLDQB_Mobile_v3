using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class FillterModal : ModalBase
	{
		[Parameter] public EventCallback OnSave { get; set; }
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }
        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        protected INavigationService navigationService { get; set; }
        protected ICatUnitAppService CatUnitAppService;

        private ItemsProviderResult<CatFilterList> CatFilterListDto;
        protected IGeoUnitAppService geoUnitAppService { get; set; }

        private ItemsProviderResult<GeoUnitDto> geoUnitDto;

        public  RecruimentInput _filter = new RecruimentInput();
        #region CatUnit
        private List<CatUnitDto> _degree { get; set; }
        private List<CatUnitDto> _career { get; set; }
        private List<CatUnitDto> _rank { get; set; }
        private List<CatUnitDto> _formOfWork { get; set; }
        private List<CatUnitDto> _experience { get; set; }
        private List<CatUnitDto> _salary { get; set; }
        private List<CatUnitDto> _staffSize { get; set; }
        private List<GeoUnitDto> _workSite { get; set; }
        public List<GeoUnitDto> WorkSite
        {
            get => _workSite;
            set => _workSite = value;
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
        #endregion
      
        public FillterModal()
		{
            geoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            navigationService = DependencyResolver.Resolve<INavigationService>();
        }

		

		public override string ModalId => "update-candidate";

		private async Task RefreshList()
		{
			await OnInitializedAsync();
			StateHasChanged();
		}
		

		
		public async Task OpenFor()
		{
			await Show();
		}

	
		private async Task Save()
		{
            await Hide();
            await OnSave.InvokeAsync();
        }

        #region GetAPI

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

        #region

        public CatFilterList catFilterList { get; set; }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {
                 _degree = catUnit.Degree;
                 _career = catUnit.Career;
                 _experience = catUnit.Experience;
                 _formOfWork = catUnit.FormOfWork;
                 _rank = catUnit.Rank;

                 var filterList = new CatFilterList()
                 {
                     Degree = _degree,
                     Career = _career,
                     Experience = _experience,
                     FormOfWork = _formOfWork,
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
        #endregion
        #endregion

    }
}
