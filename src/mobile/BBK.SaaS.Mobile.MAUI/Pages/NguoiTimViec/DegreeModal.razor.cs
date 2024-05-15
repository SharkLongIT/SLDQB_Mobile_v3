using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class DegreeModal : ModalBase
    {
        public override string ModalId => "degree-modal";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        private List<CatUnitDto> _degree { get; set; }
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;

        private readonly RecruimentInput _filter = new RecruimentInput();

        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        public DegreeModal() 
        {
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();

        }
        public List<CatUnitDto> Degree
        {
            get => _degree;
            set => _degree = value;
        }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {

                 _degree = catUnit.Degree;


                 var filterList = new CatFilterList()
                 {
                     Degree = _degree,
                 };

                 CatFilterListDto = new ItemsProviderResult<CatFilterList>(
                     items: new List<CatFilterList> { filterList },
                     totalItemCount: 1
                 );
             });

            await UserDialogsService.UnBlock();
            return CatFilterListDto;


        }
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
        public long DegreeId { get; set; }
        public string DegreeName { get; set; }
        public async void selectedValue(ChangeEventArgs args)
        {
            try
            {
                if (args.Value != null)
                {
                    long select = Convert.ToInt64(args.Value);
                    DegreeId = select;
                    var selectedWorkSite = Degree.FirstOrDefault(item => item.Id == DegreeId);
                    if (selectedWorkSite != null)
                    {
                        DegreeName = selectedWorkSite.DisplayName;
                    }
                    else
                    {
                        DegreeName = string.Empty;
                    }
                }
                await OnSave.InvokeAsync();
                await Hide();

            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }
        }

    }
}
