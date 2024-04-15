using Abp.UI;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Pages.ViecTimNguoi
{
    public partial class CareerModal : ModalBase
    {
        public override string ModalId => "career-modal";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        private List<CatUnitDto> _career { get; set; }
        protected ICatUnitAppService CatUnitAppService;
        private ItemsProviderResult<CatFilterList> CatFilterListDto;

        private readonly RecruimentInput _filter = new RecruimentInput();

        private Virtualize<CatFilterList> CatFilterListContainer { get; set; }
        public List<CatUnitDto> Career
        {
            get => _career;
            set => _career = value;
        }
        public CareerModal() {
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
               
                 _career = catUnit.Career;
                

                 var filterList = new CatFilterList()
                 {
                     Career = _career,
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
        public long CareerId { get; set; }
        public string CareerName { get; set; }
        public async void selectedValue(ChangeEventArgs args)
        {
            try
            {
                if (args.Value != null)
                {
                    long select = Convert.ToInt64(args.Value);
                    CareerId = select;
                    var selectedWorkSite = Career.FirstOrDefault(item => item.Id == CareerId);
                    if (selectedWorkSite != null)
                    {
                        CareerName = selectedWorkSite.DisplayName;
                    }
                    else
                    {
                        CareerName = string.Empty;
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
