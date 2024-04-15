using Abp.Application.Services.Dto;
using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Geographies;
using BBK.SaaS.Mdls.Category.Geographies.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.NhaTuyenDung;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class UpdateInforNTDModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }

        protected IRecruiterAppService RecruiterAppService;
        protected IGeoUnitAppService GeoUnitAppService;
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<GeoUnitDto> geoUnitDto;
        private Virtualize<GeoUnitDto> GeoUnitContainer { get; set; }

        protected NhaTuyenDungModel Model = new();

        protected RecruiterEditDto recruiterEditDto = new();

        public UpdateInforNTDModal()
        {
            RecruiterAppService = DependencyResolver.Resolve<IRecruiterAppService>();
            GeoUnitAppService = DependencyResolver.Resolve<IGeoUnitAppService>();
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
        }

        protected List<GeoUnitDto> _province;
        protected List<GeoUnitDto> _district;

        private CatFilterList catFilterLists;
        private DateTime DateOfEstablishment;
        private long SphereOfActivityId;

        public string NameCandidate;
        private List<GeoUnitDto> ListProvince { get; set; }
        private List<GeoUnitDto> ListAllGeoUnitDto { get; set; }
        private List<GeoUnitDto> ListDistrict { get; set; }
        private List<GeoUnitDto> ListVillage { get; set; }
        public List<GeoUnitDto> Province
        {
            get => _province;
            set => _province = value;
        }
        public List<GeoUnitDto> District
        {
            get => _district;
            set => _district = value;
        }

        public override string ModalId => "update-recruiter";


        public async void selectedValue(ChangeEventArgs args)
        {

            long select = Convert.ToInt64(args.Value);
            recruiterEditDto.ProvinceId = select;
            ListDistrict = ListAllGeoUnitDto.Where(x => x.ParentId == recruiterEditDto.ProvinceId).ToList();
            recruiterEditDto.DistrictId = ListDistrict.First().Id;
            ListVillage = ListAllGeoUnitDto.Where(x => x.ParentId == recruiterEditDto.DistrictId).ToList();
            if(ListVillage != null && ListVillage.Count() > 0)
            {
                recruiterEditDto.VillageId = ListVillage.First().Id;
            }
            StateHasChanged();
        }
        public async void changeDistrict(ChangeEventArgs args)
        {
            long select = Convert.ToInt64(args.Value);
            recruiterEditDto.DistrictId = select;
            ListVillage = ListAllGeoUnitDto.Where(x => x.ParentId == recruiterEditDto.DistrictId).ToList();
            if (ListVillage != null && ListVillage.Count() > 0)
            {
                recruiterEditDto.VillageId = ListVillage.First().Id;
            }
            StateHasChanged();
        }


        private async Task LoadFillter()
        {
            await WebRequestExecuter.Execute(
                       async () => await CatUnitAppService.GetFilterList(),
                       async (catUnit) =>
                       {
                           catFilterLists = catUnit;
                       }
                   );
        }

        private async Task<List<GeoUnitDto>> LoadProvince(long Province)
        {

            List<GeoUnitDto> geoUnitDtos = new List<GeoUnitDto>();
            await WebRequestExecuter.Execute(
                async () => await GeoUnitAppService.GetAll(),
                async (result) =>
                {
                    ListAllGeoUnitDto = result;
                    geoUnitDtos = result.Where(x => x.ParentId == null).ToList();
                    if (Province == null || Province == 0)
                    {
                        recruiterEditDto.ProvinceId = geoUnitDtos.FirstOrDefault().Id;
                    }
                    ListProvince = geoUnitDtos;
                    ListDistrict = ListAllGeoUnitDto.Where(x => x.ParentId == recruiterEditDto.ProvinceId).ToList();
                    ListVillage = ListAllGeoUnitDto.Where(x => x.ParentId == recruiterEditDto.DistrictId).ToList();

                }
            );

            return ListProvince;
        }
        public async Task OpenFor(NhaTuyenDungModel NhaTuyenDungModel)
        {
            try
            {

                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                            Model = new NhaTuyenDungModel();
                            Model = ObjectMapper.Map<NhaTuyenDungModel>(NhaTuyenDungModel);
                            var recruiter = await RecruiterAppService.GetRecruiterForEdit(new NullableIdDto<long> { Id = NhaTuyenDungModel.Recruiter.UserId });

                            recruiterEditDto = recruiter.Recruiter;
                            if(recruiterEditDto.DateOfEstablishment != null)
                            {
                                DateOfEstablishment = recruiterEditDto.DateOfEstablishment.Value;
                            }
                            if(recruiterEditDto.SphereOfActivityId != null)
                            {
                                SphereOfActivityId = recruiterEditDto.SphereOfActivityId.Value;
                            }
                            await LoadProvince(recruiterEditDto.ProvinceId);
                            await LoadFillter();
                          //  await IJSRuntime.InvokeVoidAsync("scrollToBottom");
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
        private async Task<bool> ValidateInput()
        {
            if (string.IsNullOrEmpty(recruiterEditDto.CompanyName))
            {
                await UserDialogsService.AlertWarn(@L("Tên công ty không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(recruiterEditDto.ContactName))
            {
                await UserDialogsService.AlertWarn(@L("Tên người liên hệ không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(recruiterEditDto.ContactEmail))
            {
                await UserDialogsService.AlertWarn(@L("Email người liên hệ không được để trống"));
                return false;
            }
            if (recruiterEditDto.HumanResSizeCatId == 0  || recruiterEditDto.HumanResSizeCatId == null)
            {
                await UserDialogsService.AlertWarn(@L("Quy mô nhân sự không được để trống"));
                return false;
            }
            if (recruiterEditDto.SphereOfActivityId == 0  || recruiterEditDto.SphereOfActivityId == null)
            {
                await UserDialogsService.AlertWarn(@L("Lĩnh vực hoạt động không được để trống"));
                return false;
            }
            if (recruiterEditDto.ProvinceId == 0 || recruiterEditDto.ProvinceId == null)
            {
                await UserDialogsService.AlertWarn(@L("Tỉnh thành phố không được để trống"));
                return false;
            }
            if (recruiterEditDto.DistrictId == 0 || recruiterEditDto.DistrictId == null)
            {
                await UserDialogsService.AlertWarn(@L("Quận huyện không được để trống"));
                return false;
            }
              if (recruiterEditDto.VillageId == 0 || recruiterEditDto.VillageId == null)
            {
                await UserDialogsService.AlertWarn(@L("Phường xã không được để trống"));
                return false;
            }
            if (string.IsNullOrEmpty(recruiterEditDto.Address))
            {
                await UserDialogsService.AlertWarn(@L("Địa chỉ không được để trống"));
                return false;
            }



            return true;

        }
        private async Task Save()
        {
            if (! await ValidateInput())
            {
                return; 
            }

            recruiterEditDto.DateOfEstablishment = DateOfEstablishment;
            recruiterEditDto.SphereOfActivityId = SphereOfActivityId;
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                async () => await RecruiterAppService.UpdateRecruiterForMobile(recruiterEditDto),
                async (result) =>
                {
                    await UserDialogsService.AlertSuccess(L("Cập nhật thành công"));
                    await Hide();
                    await OnSave.InvokeAsync();
                }
               );
            });

        }
    }
}