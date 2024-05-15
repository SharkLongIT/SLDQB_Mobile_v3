using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class FilterNTV : SaaSMainLayoutPageComponentBase
    {
        [Parameter]
        public EventCallback<string> Callback { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        bool WorkCheck;
        bool ExperienceCheck;
        bool CareerCheck;
        bool DegreeCheck;
        bool SalaryCheck;

        public FilterNTV()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();

        }
        #region Khu vực 
        protected PlaceModal PlaceModal { get; set; }

        private long WorkSiteId;

        private string WorkSiteName;
        public async Task OpenPlace()
        {
            await PlaceModal.OpenFor();

            //await MethodToCallOnClick.InvokeAsync(null);
        }

        private async Task RefreshPlace()
        {
            WorkSiteName = PlaceModal.WorkSiteName;
            WorkCheck = true;
            WorkSiteId = PlaceModal.WorkSiteId;
        }

        public async Task DeleteWork()
        {
            WorkSiteName = "";
            WorkCheck = false;
            WorkSiteId = 0;
        }

        #endregion
        #region Kinh nghiệm
        private long ExperienceId;
        private string ExperienceName;
        protected ExperienceModal ExperienceModal { get; set; }
        public async Task OpenExperience()
        {
            await ExperienceModal.OpenFor();
        }
        private async Task RefreshExperience()
        {
            ExperienceName = ExperienceModal.ExperienceName;
            ExperienceCheck = true;
            ExperienceId = ExperienceModal.ExperienceId;
        }
        public async Task DeleteExperience()
        {
            ExperienceName = "";
            ExperienceCheck = false;
            ExperienceId = 0;
        }
        #endregion
        #region Nghành nghề 
        private long CareerId;
        private string CareerName;

        protected CareerModal CareerModal { get; set; }
        public async Task OpenCareer()
        {
            await CareerModal.OpenFor();
        }
        private async Task RefreshCareer()
        {
            CareerName = CareerModal.CareerName;
            CareerCheck = true;
            CareerId = CareerModal.CareerId;
        }
        public async Task DeleteCareer()
        {
            CareerName = "";
            CareerCheck = false;
            CareerId = 0;
        }
        #endregion


        #region Bằng cấp 
        private long DegreeId;
        private string DegreeName;

        protected DegreeModal DegreeModal { get; set; }
        public async Task OpenDegree()
        {
            await DegreeModal.OpenFor();
        }
        private async Task RefeshDegree()
        {
            DegreeName = DegreeModal.DegreeName;
            DegreeCheck = true;
            DegreeId = DegreeModal.DegreeId;
        }
        public async Task DeleteDegree()
        {
            DegreeName = "";
            DegreeCheck = false;
            DegreeId = 0;
        }
        #endregion

        #region Mức lương
        private decimal? SalaryMin;
        private decimal? SalaryMax;
        protected SalaryModal SalaryModal { get; set; }
        public async Task OpenSalary()
        {
            await SalaryModal.OpenFor();
        }
        private async Task RefeshSalary()
        {

            SalaryCheck = true;
            if (SalaryModal.SalaryMin == null && SalaryModal.SalaryMax == null) { 
                SalaryCheck  = false;
            }
            SalaryMin = SalaryModal.SalaryMin;
            SalaryMax = SalaryModal.SalaryMax;
        }
        private async Task DeleteSalary()
        {
            SalaryMin = null;
            SalaryCheck = false;
            SalaryMax = null;
        }
        #endregion

        public async Task UriFilter()
        {
            navigationService.NavigateTo($"NguoiTimViec?Career={CareerId}&WorkSite={WorkSiteId}&Experience={ExperienceId}&Degree={DegreeId}&SalaryMin={SalaryMin}&SalaryMax={SalaryMax}");
        }
    }
}
