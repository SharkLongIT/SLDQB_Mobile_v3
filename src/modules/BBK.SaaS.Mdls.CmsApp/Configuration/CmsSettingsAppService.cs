using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Runtime.Session;
using BBK.SaaS.Mdls.Cms.Configuration.Dto;

namespace BBK.SaaS.Mdls.Cms.Configuration
{
	public class CmsSettingsAppService : SaaSAppServiceBase, ICmsSettingsAppService
	{
		#region Get Settings

		public async Task<CmsSettingsEditDto> GetAllSettings()
		{
			CmsSettingsEditDto settings = new()
			{
				General = new CmsGeneralSettingsEditDto()
				{
					FooterCusomHtml = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.FooterCustomHTML),
					HeaderCusomHtml = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.HeaderCustomHTML),
					MetaTitle = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.MetaTitle),
					MetaDescription = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.MetaDescription),
					MetaKeywords = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.MetaKeywords),
					OgTitle = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.OgTitle),
					OgDescription = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.OgDescription),
					OgImageUrl = await SettingManager.GetSettingValueAsync(CmsAppSettings.General.OgImageUrl),
				},
				RobotsTxt = await GetCmsRobotsTxtSettings()
			};

			return settings;
		}

		//private async Task<CmsGeneralSettingsEditDto> GetCmsGeneralSettings()
		//{

		//}

		private async Task<CmsRobotsTxtSettingsEditDto> GetCmsRobotsTxtSettings()
		{
			return new CmsRobotsTxtSettingsEditDto()
			{
				AllowSiteMapXml = await SettingManager.GetSettingValueAsync<bool>(CmsAppSettings.RobotsTxt.AllowSiteMapXml),
				DisallowPath = await SettingManager.GetSettingValueAsync(CmsAppSettings.RobotsTxt.DisallowPath)
			};
		}
		#endregion

		public async Task UpdateAllSettings(CmsSettingsEditDto input)
		{
			await UpdateCmsGeneralSettingsAsync(input.General);
			//await UpdateCmsRobotSettingsAsync(input.General);
		}

		private async Task UpdateCmsGeneralSettingsAsync(CmsGeneralSettingsEditDto input)
		{
			await SettingManager.ChangeSettingForTenantAsync(
				AbpSession.GetTenantId(),
				CmsAppSettings.General.FooterCustomHTML,
				input.FooterCusomHtml
			);

			await SettingManager.ChangeSettingForTenantAsync(
				AbpSession.GetTenantId(),
				CmsAppSettings.General.HeaderCustomHTML,
				input.HeaderCusomHtml
			);
		}

		//private async Task UpdateCmsRobotSettingsAsync(CmsRobotsTxtSettingsEditDto input)
		//{
		//	await SettingManager.ChangeSettingForTenantAsync(
		//		AbpSession.GetTenantId(),
		//		CmsAppSettings.General.FooterCustomHTML,
		//		input.
		//	);

		//	await SettingManager.ChangeSettingForTenantAsync(
		//		AbpSession.GetTenantId(),
		//		CmsAppSettings.General.HeaderCustomHTML,
		//		input.FooterCusomHtml
		//	);
		//}
	}
}
