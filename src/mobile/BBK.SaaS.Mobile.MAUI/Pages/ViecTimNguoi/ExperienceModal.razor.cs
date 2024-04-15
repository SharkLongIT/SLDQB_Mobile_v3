using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class ExperienceModal : ModalBase
    {
        public override string ModalId => "ex-modal";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;


        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        private readonly RecruimentInput _filter = new RecruimentInput();

        private List<CatUnitDto> _experience { get; set; }
        public List<CatUnitDto> Experience
        {
            get => _experience;
            set => _experience = value;
        }
        public ExperienceModal() {
            CatUnitAppService = DependencyResolver.Resolve<ICatUnitAppService>();
        }
        #region

        public CatFilterList catFilterList { get; set; }
        private async ValueTask<ItemsProviderResult<CatFilterList>> LoadFilter(ItemsProviderRequest request)
        {
            await WebRequestExecuter.Execute(
             async () => await CatUnitAppService.GetFilterList(),

             async (catUnit) =>
             {
              
                 _experience = catUnit.Experience;

                 var filterList = new CatFilterList()
                 {
                     Experience = _experience,
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

        public async Task OpenFor()
        {
            try
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
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
        public long ExperienceId { get; set; }
        public string ExperienceName { get; set; }
        public async void selectedValue(ChangeEventArgs args)
        {
            try
            {
                if (args.Value != null)
                {
                    long select = Convert.ToInt64(args.Value);
                    ExperienceId = select;
                    var selectedWorkSite = Experience.FirstOrDefault(item => item.Id == ExperienceId);
                    if (selectedWorkSite != null)
                    {
                        ExperienceName = selectedWorkSite.DisplayName;
                    }
                    else
                    {
                        ExperienceName = string.Empty;
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
