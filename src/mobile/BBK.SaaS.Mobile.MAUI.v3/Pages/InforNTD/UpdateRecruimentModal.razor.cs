using BBK.SaaS.ApiClient;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BBK.SaaS.Services.Navigation;
using BBK.SaaS.Mobile.MAUI.Models.ViecTimNguoi;
using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using System.Text.RegularExpressions;
using Microsoft.Maui.ApplicationModel.Communication;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class UpdateRecruimentModal : ModalBase
    {
        public INavigationService navigationService { get; set; }
        public ICatUnitAppService CatUnitAppService { get; set; }
        public IGeoUnitAppService GeoUnitAppService { get; set; }
        public IRecruitmentAppService recruitmentAppService { get; set; }
        public IRecruiterAppService recruiterAppService { get; set; }
        public IApplicationContext ApplicationContext { get; set; }
        public override string ModalId => "update-recruitment";
        private bool _isInitialized;
        private string DistrictId;
        private long ProvinceId;
        private string ProvinceName;
        private string DistricName;
        protected RecruitmentModel Model = new RecruitmentModel();
        protected RecruitmentDto recruitment = new();
        [Parameter] public EventCallback OnSave { get; set; }
        string FormOfWorkName;
        private MarkupString JobDescText;
        private MarkupString JobRequirementDescText;
        private MarkupString BenefitDescText;

        protected List<CatUnitDto> _jobCatUnit;
        public CatFilterList CatFilterList;

        private List<GeoUnitDto> ListProvince { get; set; }
        private List<GeoUnitDto> ListAllGeoUnitDto { get; set; }
        private List<GeoUnitDto> ListDistrict { get; set; }

        private string emailMessage ;

        public string EmailMessage
        {
            get => emailMessage;
            set
            {
                if (emailMessage != value)
                {
                    //emailMessage = ((MarkupString)value).ToString();
                    emailMessage = System.Web.HttpUtility.HtmlDecode(emailMessage);
                }
            }

        }

        public UpdateRecruimentModal()
        {
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
            GeoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            recruitmentAppService = DependencyResolver.Resolve<IRecruitmentAppService>();
            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            recruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();
        }
        public async Task OpenFor(RecruitmentDto recruitmentDto)
        {
            var recruitmentId = await recruitmentAppService.GetDetail(recruitmentDto.Id.Value);
            _isInitialized = false;
            try
            {

                await SetBusyAsync(async () =>
                {

                    Model = ObjectMapper.Map<RecruitmentModel>(recruitmentId);
                    Model.ProvinceId = recruitmentId.ProvinceId;
                    Model.DistrictCode = recruitmentId.DistrictCode;
                    //Model.JobDesc = JobDescText.Value;
                    //JobDescText = new MarkupString(Model.JobDesc);
                    //Model.JobRequirementDesc = JobRequirementDescText.Value;
                    //JobRequirementDescText = new MarkupString(Model.JobRequirementDesc);
                    //Model.BenefitDesc = BenefitDescText.Value;
                   // BenefitDescText = new MarkupString(Model.BenefitDesc);

                    recruitment = ObjectMapper.Map<RecruitmentDto>(Model);
                    //recruitment.JobDesc = JobDescText.Value;
                    //recruitment.JobRequirementDesc = JobRequirementDescText.Value;
                    //recruitment.BenefitDesc = BenefitDescText.Value;
                    await WebRequestExecuter.Execute(
                        async () => await CatUnitAppService.GetFilterList(),
                        async (result) =>
                        {
                            CatFilterList = result;
                           
                            _isInitialized = true;
                        }
                    );

                });

                recruitment.JobDesc = HTMLToText(recruitment.JobDesc);
                recruitment.JobRequirementDesc = HTMLToText(recruitment.JobRequirementDesc);
                recruitment.BenefitDesc = HTMLToText(recruitment.BenefitDesc);
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException(ex.Message);
            }
            await LoadProvince(recruitmentId.ProvinceId);
            await Show();
        }

        public string HTMLToText(string HTMLCode)
        {
            //// Remove new lines since they are not visible in HTML  
            //HTMLCode = HTMLCode.Replace("\n", " ");
            //// Remove tab spaces  
            //HTMLCode = HTMLCode.Replace("\t", " ");
            // Remove multiple white spaces from HTML  
            HTMLCode = Regex.Replace(HTMLCode, "\\s+", " ");
            // Remove HEAD tag  
            HTMLCode = Regex.Replace(HTMLCode, "<head.*?</head>", ""
                                , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // Remove any JavaScript  
            HTMLCode = Regex.Replace(HTMLCode, "<script.*?</script>", ""
              , RegexOptions.IgnoreCase | RegexOptions.Singleline);
            // Replace special characters like &, <, >, " etc.  
            StringBuilder sbHTML = new StringBuilder(HTMLCode);
            // Note: There are many more special characters, these are just  
            // most common. You can add new characters in this arrays if needed  
            string[] OldWords = {"&nbsp;", "&amp;", "&quot;", "&lt;",
   "&gt;", "&reg;", "&copy;", "&bull;", "&trade;","&#39;"};
            string[] NewWords = { " ", "&", "\"", "<", ">", "Â®", "Â©", "â€¢", "â„¢", "\'" };
            for (int i = 0; i < OldWords.Length; i++)
            {
                sbHTML.Replace(OldWords[i], NewWords[i]);
            }
            // Check if there are line breaks (<br>) or paragraph (<p>)  
            sbHTML.Replace("<br>", "\n<br>");
            sbHTML.Replace("<br ", "\n<br ");
            sbHTML.Replace("<p ", "\n<p ");
            // Finally, remove all HTML tags and return plain text  
            return System.Text.RegularExpressions.Regex.Replace(
              sbHTML.ToString(), "<[^>]*>", "");
        }

        public async void selectedValue(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            ProvinceId = select;
            //Model.ProvinceId = select;

            ListDistrict = ListAllGeoUnitDto.Where(x => x.ParentId == select).ToList();
            List<string> idList = new List<string>();

            foreach (var item in ListDistrict)
            {
                idList.Add(item.Code.ToString());
            }
            foreach (var id in idList)
            {
                DistrictId = id;
            }

        }

        private async Task<List<GeoUnitDto>> LoadProvince(long ProvinceId)
        {

            List<GeoUnitDto> geoUnitDtos = new List<GeoUnitDto>();
            await WebRequestExecuter.Execute(
                async () => await GeoUnitAppService.GetAll(),
                async (result) =>
                {
                    ListAllGeoUnitDto = result;
                    geoUnitDtos = result.Where(x => x.ParentId == null).ToList();
                    ListProvince = geoUnitDtos;
                    ListDistrict = ListAllGeoUnitDto.Where(x=>x.ParentId == ProvinceId).ToList();
                }
            );

            return ListProvince;
        }
        private async Task<bool> ValidateInput()
        {
            if (string.IsNullOrEmpty(Model.Title))
            {
                await UserDialogsService.AlertWarn(@L("Chức danh không được để trống"));
                return false;
            }
            if (Model.JobCatUnitId == null || Model.JobCatUnitId <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn nghề nghiệp"));
                return false;
            }
            if (Model.FormOfWork == null || Model.FormOfWork <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn hình thức làm việc"));
                return false;
            }
            if (Model.Degree == null || Model.Degree <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn bằng cấp"));
                return false;
            }

            if (Model.Experience == null || Model.Experience <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn số năm kinh nghiệm"));
                return false;
            }
            if (Model.Rank == null || Model.Rank <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng chọn cấp bậc"));
                return false;
            }
            if (Model.MinAge == null || Model.MinAge <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Độ tuổi tối thiếu không được để trống"));
                return false;
            }
            if (Model.MaxAge == null || Model.MaxAge <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Độ tuổi tối đa không được để trống"));
                return false;
            }
            if (Model.MinAge > Model.MaxAge)
            {
                await UserDialogsService.AlertWarn(@L("Độ tuổi tối thiểu không được lớn hơn độ tuổi tối đa"));
                return false;
            }
            if (Model.NumberOfRecruits == null || Model.NumberOfRecruits <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Số lượng tuyển không được để trống"));
                return false;
            }
            if (Model.DeadlineSubmission < DateTime.Now)
            {
                await UserDialogsService.AlertWarn(@L("Hạn nộp hồ sơ không được nhỏ hơn thời gian hiện tại "));
                return false;
            }
            //if (ProvinceId == null || ProvinceId <= 0)
            //{
            //    await UserDialogsService.AlertWarn(@L("Vui lòng chọn Tỉnh/Thành phố"));
            //    return false;
            //}
            if (Model.MinSalary == null || Model.MinSalary <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng nhập mức lương tối thiểu"));
                return false;
            }
            if (Model.MaxSalary == null || Model.MaxSalary <= 0)
            {
                await UserDialogsService.AlertWarn(@L("Vui lòng nhập mức lương tối thiểu"));
                return false;
            }
            if (Model.MaxSalary <= Model.MinSalary)
            {
                await UserDialogsService.AlertWarn(@L("Mức lương tối đa phải lớn hơn mức lương tối thiểu"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.JobDesc))
            {
                await UserDialogsService.AlertWarn(@L("Mô tả công việc không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.JobRequirementDesc))
            {
                await UserDialogsService.AlertWarn(@L("Yêu cầu công việc không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(Model.BenefitDesc))
            {
                await UserDialogsService.AlertWarn(@L("Quyền lợi không được để trống"));
                return false;
            }
            if (Model.FullName == null || string.IsNullOrEmpty(Model.FullName))
            {
                await UserDialogsService.AlertWarn("Họ và tên không được để trống!");
                return false;
            }
            if (Model.Email == null || string.IsNullOrEmpty(Model.Email))
            {
                await UserDialogsService.AlertWarn("Email không được để trống!");
                return false;
            }
            string emailPattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            if (!Regex.IsMatch(Model.Email, emailPattern))
            {
                await UserDialogsService.AlertWarn("Email không hợp lệ!");
                return false;
            }
            if (Model.PhoneNumber == null || string.IsNullOrEmpty(Model.PhoneNumber))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không được để trống!");
                return false;
            }
            string phonePattern = @"^0[0-9]{9,10}$";
            if (!Regex.IsMatch(Model.PhoneNumber, phonePattern))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không hợp lệ! Vui lòng kiểm tra lại.");
                return false;
            }
            if (Model.Address == null || string.IsNullOrEmpty(Model.Address))
            {
                await UserDialogsService.AlertWarn("Địa chỉ liên hệ không được để trống!");
                return false;
            }
            return true;

        }

        private async Task SaveJob()
        {
            await SetBusyAsync(async () =>
            {
                var recruiterId = await recruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = ApplicationContext.LoginInfo.User.Id });
                if (recruiterId != null)
                {
                    recruitment.RecruiterId = recruiterId.Recruiter.Id.Value;
                }

                if (!await ValidateInput())
                {
                    return;
                }
                if (DistrictId == null)
                {
                    recruitment.DistrictCode = Model.DistrictCode;
                }
                else
                {
                    recruitment.DistrictCode = DistrictId;
                }
                if (ProvinceId == 0)
                {
                     recruitment.ProvinceId = Model.ProvinceId;
                }
                else
                {
                      recruitment.ProvinceId = ProvinceId;
                }
                //Model.AddressName = Model.Address + "," + DistricName + "," + ProvinceName; 
                //Model.RecruiterId = ApplicationContext.LoginInfo.User.Id;
                recruitment.TenantId = 1;
                //Model.WorkAddress = Model.Address;
                await WebRequestExecuter.Execute(
                  async () => await recruitmentAppService.Update(recruitment),
                  async (result) =>
                  {
                      await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                      await Hide();
                      await OnSave.InvokeAsync();
                      /// Model.Id = result;
                  }
               );
                //        navigationService.NavigateTo($"ChiTietBTD?Id={Model.Id}");
            });


        }
    }
}