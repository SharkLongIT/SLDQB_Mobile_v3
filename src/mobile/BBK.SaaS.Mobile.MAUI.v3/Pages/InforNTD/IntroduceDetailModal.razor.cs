using Abp.UI;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mobile.MAUI.Models.Introduce;
using BBK.SaaS.Mobile.MAUI.Shared;
using Microsoft.AspNetCore.Components;

namespace BBK.SaaS.Mobile.MAUI.Pages.InforNTD
{
    public partial class IntroduceDetailModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }
        public override string ModalId => "introduce-detail";
        //protected IntroduceEditModel Model = new IntroduceEditModel();
        string Title;
        string FullName;
        string Email;
        string Phone;
        string Description;
        int Status;
        public IntroduceDetailModal()
        {

        }
        public async Task OpenFor(IntroduceEditDto introduce)
        {
          
            try
            {
                await SetBusyAsync(async () =>
                {
                    Title = introduce.Article.Title;
                    FullName = introduce.FullName;
                    Email = introduce.Email;
                    Phone = introduce.Phone;
                    Description = introduce.Description;
                    Status = introduce.Status;

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

    }
}
