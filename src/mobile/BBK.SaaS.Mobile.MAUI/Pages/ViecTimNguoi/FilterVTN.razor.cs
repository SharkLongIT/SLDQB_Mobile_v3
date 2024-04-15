using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class FilterVTN : SaaSMainLayoutPageComponentBase
    {
        [Parameter]
        public EventCallback<string> Callback { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected NavMenu NavMenu { get; set; }
        bool WorkCheck;
        bool ExperienceCheck;
        bool CareerCheck;
        bool SalaryCheck;
        
        public FilterVTN() 
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Tìm kiếm"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("FilterVTN") + "FilterVTN".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            if (q1["WorkSiteId"] != null)
            {
                
                WorkSiteId = int.Parse(q1["WorkSiteId"]);
            }
            if (q1["WorkSiteName"] != null)
            {
                WorkSiteName = (q1["WorkSiteName"]);
            }
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

        //private async Task OnClick() => await MethodToCallOnClick.InvokeAsync(null);

        #region Kinh nghiệm
        private long ExperienceId;
        private string ExperienceName ;
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
            if (SalaryModal.SalaryMin == null && SalaryModal.SalaryMax == null)
            {
                SalaryCheck = false;
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
            if (SalaryModal.SalaryMin.HasValue)
            {
                SalaryMin = SalaryModal.SalaryMin.Value;
            }
            else
            {
                SalaryMin = null;
            }
            if (SalaryModal.SalaryMax.HasValue)
            {
                SalaryMax = SalaryModal.SalaryMax.Value;
            }
            else
            {
                  SalaryMax= null;
            }
            navigationService.NavigateTo($"ViecTimNguoi?Job={CareerId}&WorkSite={WorkSiteId}&Experience={ExperienceId}&SalaryMin={SalaryMin}&SalaryMax={SalaryMax}");
        }
    }
}
