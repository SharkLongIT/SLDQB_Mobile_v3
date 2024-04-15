using Abp.Threading;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Mobile.MAUI.Services.Article;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using BBK.SaaS.Core.Dependency;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNLD
{
    public partial class ThongTinViecLam : SaaSMainLayoutPageComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        protected INavigationService navigationService { get; set; }
        protected IRecruitmentAppService recruitmentAppService { get; set; }
        protected IArticleService articleService { get; set; }
        protected IApplicationContext ApplicationContext { get; set; }


        [Parameter]
        public long Id { get; set; } = 0;
        protected ViecTimNguoiModel Model = new ViecTimNguoiModel();
        private DateTime date;
        public string HumanResSizeCat;
        public string Experiences;
        private MarkupString htmlContent1;
        private MarkupString htmlContent2;
        private MarkupString htmlContent3;

        public ThongTinViecLam()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            articleService = DependencyResolver.Resolve<IArticleService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();

        }
        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Thông tin việc làm"), new List<Services.UI.PageHeaderButton>());
            var querySegment = NavigationManager.Uri.Substring(NavigationManager.Uri.IndexOf("ThongTinViecLam") + "ThongTinViecLam".Length);
            var q1 = System.Web.HttpUtility.ParseQueryString(querySegment);
            //int? Id = null;

            if (q1["Id"] != null)
            {
                Id = int.Parse(q1["Id"]);
            }
            if (q1["HumanResSizeCat"] != null)
            {
                HumanResSizeCat = (q1["HumanResSizeCat"]);
            }
            if (q1["Experiences"] != null)
            {
                Experiences = (q1["Experiences"]);
            }
            RecruitmentDto recruitment = await recruitmentAppService.GetDetail(Id);

            Model = new ViecTimNguoiModel();
            Model.Id = recruitment.Id;
            Model.Title = recruitment.Title;// Vị trí , tiêu đề
            Model.MinSalary = recruitment.MinSalary;
            Model.MaxSalary = recruitment.MaxSalary;
            Model.DeadlineSubmission = recruitment.DeadlineSubmission;

            //company
            Model.RecruiterId = recruitment.Recruiter.UserId;
            Model.CompanyName = recruitment.Recruiter.CompanyName;
            Model.HumanResSizeCat = HumanResSizeCat; // Quy mô nhân sự
            //Model.WorkAddress = recruitment.RecruitmentAddress;// địa chỉ làm việc
            Model.AvatarUrl = recruitment.Recruiter.AvatarUrl;

            Model.AddressCompany = recruitment.Recruiter.Address; //địa chỉ công ty

            //Thông tin tuyển dụng:
            Model.JobCatUnitName = recruitment.JobCatUnitName;
            Model.AddressName = recruitment.AddressName;
            Model.NecessarySkills = recruitment.NecessarySkills;
            Model.Experience = Experiences; // kinh nghiệm
            Model.FormOfWork = recruitment.FormOfWork; // Hình thức làm việc
            Model.NumberOfRecruits = recruitment.NumberOfRecruits; // số lượng
            Model.GenderRequired = recruitment.GenderRequired; // yc giới tính

            htmlContent1 = new MarkupString(recruitment.JobDesc);
            htmlContent2 = new MarkupString(recruitment.JobRequirementDesc);
            htmlContent3 = new MarkupString(recruitment.BenefitDesc);


            //người liên hệ
            Model.FullName = recruitment.FullName;
            Model.Email = recruitment.Email;
            Model.PhoneNumber = recruitment.PhoneNumber;
            Model.Address = recruitment.Address;
            await UpdateImage();

        }
        public Task UpdateImage()
        {
            Model.AvatarUrl = AsyncHelper.RunSync(async () => await articleService.GetPicture(Model.AvatarUrl));
            return Task.CompletedTask;
        }
        public async Task ViewCompany(ViecTimNguoiModel recruitmentDto)
        {
            navigationService.NavigateTo($"CompanyDetail?RecruiterId={recruitmentDto.RecruiterId}");
        }


    }
}

