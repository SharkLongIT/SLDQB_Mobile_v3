using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.Localization;
using BBK.SaaS.Authorization;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Security;
using BBK.SaaS.Web.Areas.App.Models.Users;
using BBK.SaaS.Web.Areas.Profile.Models.UsersType;
using BBK.SaaS.Web.Controllers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BBK.SaaS.Web.Areas.Lib.Controllers
{
	[Area("Profile")]
	[AbpMvcAuthorize]
	public class UsersTypeController : SaaSControllerBase
	{
		private readonly IUserAppService _userAppService;
		private readonly ILanguageManager _languageManager;
		private readonly IPasswordComplexitySettingStore _passwordComplexitySettingStore;
		private readonly IOptions<UserOptions> _userOptions;

		public UsersTypeController(
			IUserAppService userAppService,
			IPasswordComplexitySettingStore passwordComplexitySettingStore,
			IOptions<UserOptions> userOptions, 
			ILanguageManager languageManager
			)
		{
			_userAppService = userAppService;
			_languageManager = languageManager;
			_passwordComplexitySettingStore = passwordComplexitySettingStore;
			_userOptions = userOptions;

		}

		[AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
		public async Task<ActionResult> IndexCandidates()
		{
			//var roles = new List<ComboboxItemDto>();

			//         if (await IsGrantedAsync(AppPermissions.Pages_Administration_Roles))
			//         {
			//             var getRolesOutput = await _roleAppService.GetRoles(new GetRolesInput());
			//             roles = getRolesOutput.Items.Select(r => new ComboboxItemDto(r.Id.ToString(), r.DisplayName)).ToList();
			//         }

			//         roles.Insert(0, new ComboboxItemDto("", L("FilterByRole")));

			//var permissions = _permissionAppService.GetAllPermissions().Items.ToList();

			var model = new UsersTypeViewModel
			{
				FilterText = Request.Query["filterText"],
				//Roles = roles,
				//Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName)
				//    .ToList(),
				OnlyLockedUsers = false
			};

			return View(model);
		}

		[AbpMvcAuthorize(AppPermissions.Pages_Administration_Users)]
		public async Task<ActionResult> IndexRecruiters()
		{
			//var roles = new List<ComboboxItemDto>();

			//         if (await IsGrantedAsync(AppPermissions.Pages_Administration_Roles))
			//         {
			//             var getRolesOutput = await _roleAppService.GetRoles(new GetRolesInput());
			//             roles = getRolesOutput.Items.Select(r => new ComboboxItemDto(r.Id.ToString(), r.DisplayName)).ToList();
			//         }

			//         roles.Insert(0, new ComboboxItemDto("", L("FilterByRole")));

			//var permissions = _permissionAppService.GetAllPermissions().Items.ToList();

			var model = new UsersTypeViewModel
			{
				FilterText = Request.Query["filterText"],
				//Roles = roles,
				//Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName)
				//    .ToList(),
				OnlyLockedUsers = false
			};

			return View(model);
		}

		//public async Task<IActionResult> Detail(long id)
		//{
		//	//if (baseLanguageName.IsNullOrEmpty())
		//	//{
		//	//	baseLanguageName = _languageManager.CurrentLanguage.Name;
		//	//}
		//	var viewModel = new RecruiterDetailViewModel();

		//	//viewModel.Languages = _languageManager.GetLanguages().ToList();

		//	return View(viewModel);
		//}

		[AbpMvcAuthorize(
			AppPermissions.Pages_Administration_Users,
			AppPermissions.Pages_Administration_Users_Create,
			AppPermissions.Pages_Administration_Users_Edit
		)]
		public async Task<PartialViewResult> CreateOrEditModal(long? id, UserTypeEnum userType)
		{
			var output = await _userAppService.GetUserForEdit(new NullableIdDto<long> { Id = id });
			var viewModel = ObjectMapper.Map<CreateOrEditUserModalViewModel>(output);
			viewModel.PasswordComplexitySetting = await _passwordComplexitySettingStore.GetSettingsAsync();
			viewModel.AllowedUserNameCharacters = _userOptions.Value.AllowedUserNameCharacters;
			ViewBag.UserType = userType;

			return PartialView("_CreateOrEditModal", viewModel);
		}
	}
}
