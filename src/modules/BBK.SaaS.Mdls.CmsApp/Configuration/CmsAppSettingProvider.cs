using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration;
using BBK.SaaS.Configuration;
using Microsoft.Extensions.Configuration;

namespace BBK.SaaS.Mdls.Cms.Configuration
{
	/// <summary>
	/// Defines settings for the application.
	/// See <see cref="AppSettings"/> for setting names.
	/// </summary>
	public class CmsAppSettingProvider : SettingProvider
	{
		private readonly IConfigurationRoot _appConfiguration;

		public CmsAppSettingProvider(IAppConfigurationAccessor configurationAccessor)
		{
			_appConfiguration = configurationAccessor.Configuration;

		}

		public override IEnumerable<SettingDefinition> GetSettingDefinitions(SettingDefinitionProviderContext context)
		{
			//// Disable TwoFactorLogin by default (can be enabled by UI)
			//context.Manager.GetSettingDefinition(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled)
			//    .DefaultValue = false.ToString().ToLowerInvariant();

			//// Change scope of Email settings
			//ChangeEmailSettingScopes(context);

			return GetGeneralSettings().Union(GetRobotsTxtSettings());
		}

		private IEnumerable<SettingDefinition> GetGeneralSettings()
		{
			return new[]
			{
				new SettingDefinition(CmsAppSettings.General.HeaderCustomHTML,
					GetFromAppSettings(CmsAppSettings.General.HeaderCustomHTML, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.FooterCustomHTML,
					GetFromAppSettings(CmsAppSettings.General.FooterCustomHTML, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.MetaTitle,
					GetFromAppSettings(CmsAppSettings.General.MetaTitle, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.MetaDescription,
					GetFromAppSettings(CmsAppSettings.General.MetaDescription, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.MetaKeywords,
					GetFromAppSettings(CmsAppSettings.General.MetaKeywords, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.OgTitle,
					GetFromAppSettings(CmsAppSettings.General.OgTitle, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.OgDescription,
					GetFromAppSettings(CmsAppSettings.General.OgDescription, ""), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.General.OgImageUrl,
					GetFromAppSettings(CmsAppSettings.General.OgImageUrl, ""), scopes: SettingScopes.Tenant),
			};
		}

		private IEnumerable<SettingDefinition> GetRobotsTxtSettings()
		{
			return new[]
			{
				new SettingDefinition(CmsAppSettings.RobotsTxt.AllowSiteMapXml,
					GetFromAppSettings(CmsAppSettings.RobotsTxt.AllowSiteMapXml, "true"), scopes: SettingScopes.Tenant),
				new SettingDefinition(CmsAppSettings.RobotsTxt.DisallowPath,
					GetFromAppSettings(CmsAppSettings.RobotsTxt.DisallowPath, "/bin/\r\n/view-resources/\r\n/install\r\n"), scopes: SettingScopes.Tenant),
			};
		}

		private string GetFromAppSettings(string name, string defaultValue = null)
		{
			return GetFromSettings("App:" + name, defaultValue);
		}

		private string GetFromSettings(string name, string defaultValue = null)
		{
			return _appConfiguration[name] ?? defaultValue;
		}
	}
}
