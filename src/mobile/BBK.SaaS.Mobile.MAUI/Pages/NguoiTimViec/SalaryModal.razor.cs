using Abp.UI;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mobile.MAUI.Models;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.NguoiTimViec
{
    public partial class SalaryModal : ModalBase
    {
        public override string ModalId => "salary-modal";

        [Parameter] public EventCallback<string> OnSave { get; set; }
       public decimal? SalaryMin;
       public decimal? SalaryMax;

        protected SalaryModel Model = new();
        public SalaryModal()
        {

        }
        public async Task OpenFor()
        {
            try
            {
                await SetBusyAsync(async () =>
                {
                    await WebRequestExecuter.Execute(
                        async () =>
                        {
                           Model = new SalaryModel();
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
        private async Task Save()
        {
            //if (!await ValidateInput())
            //{
            //    return;
            //}
            await SetBusyAsync(async () =>
            {
                if (Model.SalaryMin.HasValue)
                {
                    SalaryMin = Model.SalaryMin.Value;
                }
                if (Model.SalaryMax.HasValue)
                {
                   SalaryMax = Model.SalaryMax.Value;
                }
                await WebRequestExecuter.Execute(
                async () =>
                {
                    await Hide();
                    await OnSave.InvokeAsync();
                }
               );
            });

        }
    }
}
