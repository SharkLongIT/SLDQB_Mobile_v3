using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms
{
	public class CmsPageNames
	{
		public static class Administrators
        {
            public const string ContentManagementAdmin = "Administrations.ContentMgmt";
            public const string Categories = "Administrations.ContentMgmt.Categories";
            public const string UrlRecords = "Administrations.ContentMgmt.UrlRecords";
            public const string Articles = "Administrations.ContentMgmt.Articles";
            public const string MediasMgr = "Administrations.ContentMgmt.MediasMgr";
            public const string Topics = "Administrations.ContentMgmt.Topics";
            public const string Widgets = "Administrations.ContentMgmt.Widgets";
            public const string WidgetZones = "Administrations.ContentMgmt.WidgetZones";
            public const string Settings = "Administrations.ContentMgmt.Settings";
        }
	}

    public class CmsCacheNames
    {
        /// <summary>
        /// User settings cache: AbpUserSettingsCache.
        /// </summary>
        public const string TenantWidgetZones = "CmsTenantWidgetZonesCache";
    }
}
