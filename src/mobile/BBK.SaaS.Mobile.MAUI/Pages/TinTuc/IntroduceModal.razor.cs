using Abp.UI;
using BBK.SaaS.ApiClient;
using BBK.SaaS.Core.Dependency;
using BBK.SaaS.Core.Threading;
using BBK.SaaS.LienHeHoiDap;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mobile.MAUI.Models.Introduce;
using BBK.SaaS.Mobile.MAUI.Models.TinTuc;
using BBK.SaaS.Mobile.MAUI.Shared;
using BBK.SaaS.Services.Navigation;
using Microsoft.AspNetCore.Components;
using System.Text.RegularExpressions;

namespace BBK.SaaS.Mobile.MAUI.Pages.TinTuc
{
    public partial class IntroduceModal : ModalBase
    {
        public override string ModalId => "Introduce";

        [Parameter] public EventCallback<string> OnSave { get; set; }
        protected IApplicationContext ApplicationContext;
        protected IIntroduceAppService introduceAppService;
        private readonly IntroduceSearch _filtered = new IntroduceSearch();

        protected IntroduceModel Model = new IntroduceModel();
        protected ArticleModel ArticleModel = new();
        bool _isInitialized;
        private bool IsUserLoggedIn;
        private bool ReturnLogin;

        protected INavigationService navigationService { get; set; }

        public IntroduceModal()
        {
            navigationService = DependencyResolver.Resolve<INavigationService>();

            ApplicationContext = DependencyResolver.Resolve<IApplicationContext>();
            introduceAppService = DependencyResolver.Resolve<IIntroduceAppService>();
        }

        public async Task OpenFor(ArticleModel articleModel)
        {
            try
            {
                await SetBusyAsync(async () =>
                {
                    Model = new IntroduceModel();
                    Model.ArticleId = articleModel.Id;
                    await WebRequestExecuter.Execute(
                        async () =>
                        {

                        }
                    );
                });
                await Show();
            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }

        }
        private async Task<bool> ValidateInput()
        {
            //IsUserLoggedIn = navigationService.IsUserLoggedIn();

            //if (IsUserLoggedIn == false)
            //{
            //    await UserDialogsService.AlertWarn("Bạn chưa đăng nhập!");
            //    ReturnLogin = true;
            //    return false;
            //}
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
            if (Model.Phone == null || string.IsNullOrEmpty(Model.Phone))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không được để trống!");
                return false;
            }
            string phonePattern = @"^[1-9][0-9]{8}$";
            if (!Regex.IsMatch(Model.Phone, phonePattern))
            {
                await UserDialogsService.AlertWarn("Số điện thoại không hợp lệ! Vui lòng kiểm tra lại.");
                return false;
            }
            if (Model.Description == null || string.IsNullOrEmpty(Model.Description))
            {
                await UserDialogsService.AlertWarn("Nội dung không được để trống!");
                return false;
            }

            return true;

        }

        private async Task CreateIntroduce()
        {
            try
            {
             

                await SetBusyAsync(async () =>
                {
                    if (!await ValidateInput())
                    {
                        return;
                    }
                    //Model.CreatorUserId = ApplicationContext.LoginInfo.User.Id;
                    await WebRequestExecuter.Execute(
                      async () => await introduceAppService.Create(Model),
                      async (result) =>
                      {
                          //Model = new ContactModel();
                          await UserDialogsService.AlertSuccess(L("Đã giới thiệu thành công!"));
                          await Hide();
                          await OnSave.InvokeAsync();
                      }
                   ); ;
                });


            }
            catch (Exception ex)
            {

                throw new UserFriendlyException(ex.Message);
            }


        }
    }
}
