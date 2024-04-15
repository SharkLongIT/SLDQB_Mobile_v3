using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using BBK.SaaS.Core.Dependency;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class ChiTietBTD : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        [Parameter]
        public long Id { get; set; } = 0;
        public string Experiences;
        public string JobCatUnitName;
        public string HumanResSizeCat;
        protected ViecTimNguoiModel Model = new ViecTimNguoiModel();
        private DateTime date;
        private MarkupString htmlContent1;
        private MarkupString htmlContent2;
        private MarkupString htmlContent3;

        public ChiTietBTD()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Thông tin việc làm"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ChiTietBTD") + "ChiTietBTD".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            //int? Id = null;

            if (q1["Id"] != null)
            {
                Id = int.Parse(q1["Id"]);
            }
           if (q1["Experiences"] != null)
            {
                Experiences = (q1["Experiences"]);
            }
            if (q1["JobCatUnitName"] != null)
            {
                JobCatUnitName = (q1["JobCatUnitName"]);
            }
            if (q1["HumanResSizeCat"] != null)
            {
                HumanResSizeCat = (q1["HumanResSizeCat"]);
            }
            RecruitmentDto recruitment = await recruitmentAppService.GetDetail(Id);

            Model = new ViecTimNguoiModel();
            Model.Id = recruitment.Id;
            Model.Title = recruitment.Title;// Vị trí , tiêu đề
            Model.MinSalary = recruitment.MinSalary;
            Model.MaxSalary = recruitment.MaxSalary;
            Model.DeadlineSubmission = recruitment.DeadlineSubmission;
            //company
            Model.CompanyName = recruitment.Recruiter.CompanyName;
            Model.HumanResSizeCat = HumanResSizeCat; // Quy mô nhân sự
            //Thông tin tuyển dụng:
            Model.AddressName = recruitment.AddressName;// dia chi lam viec
            Model.JobCatUnitName = JobCatUnitName; // 
            Model.Experience = Experiences; // kinh nghiệm
            Model.FormOfWorkName = recruitment.FormOfWorks.DisplayName; // Hình thức làm việc
            Model.NumberOfRecruits = recruitment.NumberOfRecruits; // số lượng tuyển
            Model.GenderRequired = recruitment.GenderRequired; // yc giới tính
            Model.Degree = recruitment.Degree;//bằng cấp
            Model.MinAge = recruitment.MinAge;
            Model.MaxAge = recruitment.MaxAge;
            Model.ProbationPeriod = recruitment.ProbationPeriod;
            //Model.JobDesc = recruitment.JobDesc;   //Mô tả công việc
            //Model.JobRequirementDesc = recruitment.JobRequirementDesc; // yêu cầu
            //Model.BenefitDesc = recruitment.BenefitDesc; // quyền lợi
            Model.NecessarySkills = recruitment.NecessarySkills;
            htmlContent1 = new MarkupString(recruitment.JobDesc);
            htmlContent2 = new MarkupString(recruitment.JobRequirementDesc);
            htmlContent3 = new MarkupString(recruitment.BenefitDesc);

            //người liên hệ
            Model.FullName = recruitment.FullName;
            Model.Email = recruitment.Email;
            Model.PhoneNumber = recruitment.PhoneNumber;
            Model.Address = recruitment.Address;
        }


    }
}
