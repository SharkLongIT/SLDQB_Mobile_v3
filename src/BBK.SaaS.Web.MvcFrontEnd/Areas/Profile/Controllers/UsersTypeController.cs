using Abp.AspNetCore.Mvc.Authorization;
using Abp.HtmlSanitizer;
using Abp.Localization;
using Abp.UI;
using Abp.Web.Models;
using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Web.Areas.Profile.Models.Recruiters;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType0;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
    [Area("Profile")]
    [AbpMvcAuthorize]
    public class UsersTypeController : SaaSControllerBase
    {
        private readonly ILanguageManager _languageManager;
        private readonly IUserTypeAppService _userTypeAppService;

        public UsersTypeController(
            ILanguageManager languageManager,
            IUserTypeAppService userTypeAppService
            )
        {
            _languageManager = languageManager;
            _userTypeAppService = userTypeAppService;

        }

        [HttpPost]
        //[UnitOfWork(IsolationLevel.ReadUncommitted)]
        [HtmlSanitizer]
        public async Task<ActionResult> UpdateUserInfoFormSubmit(UserTypeInfoViewModel model)
        {
            try
            {
                await _userTypeAppService.Update(AbpSession.TenantId.Value, new Authorization.Users.Dto.UserEditDto()
                {
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    EmailAddress = "unknow@bbk.com"
                });
                //await _userTypeAppService.UpdateUserAsync(new Mdls.Profile.Authorization.Dto.UpdateUserTypeInput()
                //{
                //	User = new Mdls.Profile.Authorization.Dto.UserTypeDto()
                //	{
                //		Name = model.Name,
                //		PhoneNumber = model.PhoneNumber
                //	}
                //}); ;

                //return View();
            }
            catch (UserFriendlyException ex)
            {
                //ViewBag.UseCaptcha = !model.IsExternalLogin && UseCaptchaOnRegistration();
                //ViewBag.ErrorMessage = ex.Message;

                //model.PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync();

                //return View("Register", model);
            }
            return View("RecruiterInfo");

        }

        [HttpPost]
        [HtmlSanitizer]
        public async Task<JsonResult> UpdateUserInfo(UserTypeInfoViewModel model)
        {
            try
            {
                await _userTypeAppService.Update(AbpSession.TenantId.Value, new Authorization.Users.Dto.UserEditDto()
                {
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    EmailAddress = "unknow@bbk.com"
                });

                //await _recruiterAppService.Update(model);
                return Json(Ok());
            }
            catch (Exception ex)
            {
                return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
            }
        }

    }
}
