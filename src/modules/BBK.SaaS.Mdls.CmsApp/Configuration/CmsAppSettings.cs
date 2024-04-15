using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.Configuration
{
	public class CmsAppSettings
	{
		public static class General
        {
            public const string HeaderCustomHTML = "App.Cms.General.HeaderCustomHTMLSettings";
            public const string FooterCustomHTML = "App.Cms.General.FooterCustomHTMLSettings";
            public const string MetaKeywords = "App.Cms.General.MetaKeywords";
            public const string MetaDescription = "App.Cms.General.MetaDescription";
            public const string MetaTitle = "App.Cms.General.MetaTitle";
            public const string OgTitle = "App.Cms.General.OgTitle";
            public const string OgDescription = "App.Cms.General.OgDescription";
            public const string OgImageUrl = "App.Cms.General.OgImageUrl";
        }

		public static class RobotsTxt
        {
            public const string AllowSiteMapXml = "App.Cms.Robot.AllowSiteMapXml";
            public const string DisallowPath = "App.Cms.Robot.DisallowPath";
        }
	}
}
