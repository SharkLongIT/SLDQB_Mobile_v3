using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Web.Models.RightSidebar;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Views.Shared.Components.WidgetReport
{
    public partial class WidgetReportViewComponent :  SaaSViewComponent
    {
        private readonly IRecruitmentAppService _recruitmentAppService;
        private readonly IJobApplicationAppService _jobApplicationAppService;
        private readonly IRecruiterAppService _recruiterAppService;
        public WidgetReportViewComponent(IRecruitmentAppService recruitmentAppService , IJobApplicationAppService jobApplicationAppService, IRecruiterAppService recruiterAppService)
        {
            _jobApplicationAppService = jobApplicationAppService;
            _recruiterAppService = recruiterAppService;
            _recruitmentAppService = recruitmentAppService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            int CountRecruiment =await _recruitmentAppService.CountRecruiment();

            int CountJob = await _jobApplicationAppService.CountJob();

            int CountRecruiter = await _recruiterAppService.CountRecruiter();

            RightSidebarViewModel model = new RightSidebarViewModel();
            model.CountRecruiment = CountRecruiment;
            model.CountJob = CountJob;
            model.CountRecruiter = CountRecruiter;
            return View(model);
        }
    }
}
