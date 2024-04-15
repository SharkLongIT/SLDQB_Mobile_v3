using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Configuration.Dto
{
	public class CmsGeneralSettingsEditDto
	{
		public string HeaderCusomHtml { get; set; }
		public string FooterCusomHtml { get; set; }
		public string MetaTitle { get; set; }
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string OgTitle { get; set; }
		public string OgDescription { get; set; }
		public string OgImageUrl { get; set; }
	}
}
