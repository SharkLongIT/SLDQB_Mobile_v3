using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.NguoiTimViec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.GioiThieu
{
    public partial class Index : SaaSMainLayoutPageComponentBase
    {
        public Index()
        {
        //    navigationService = DependencyResolver.Resolve<INavigationService>();
        //    jobApplicationAppService = DependencyResolver.Resolve<IJobApplicationAppService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Giới thiệu"));
        }
    }
}
