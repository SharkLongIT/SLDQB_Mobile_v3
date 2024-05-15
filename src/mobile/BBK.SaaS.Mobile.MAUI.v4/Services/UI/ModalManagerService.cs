using Abp.Dependency;
using Microsoft.JSInterop;

namespace BBK.SaaS.Mobile.MAUI.Services.UI
{
    public class ModalManagerService : ITransientDependency
    {
        public async Task Show(IJSRuntime JS, string jquerySelector)
        {
            await JS.InvokeVoidAsync("ModalCustomManagerService.show", jquerySelector);
        }

        public async Task Hide(IJSRuntime JS, string jquerySelector)
        {
            await JS.InvokeVoidAsync("ModalCustomManagerService.hide", jquerySelector);
        }
    }
}
