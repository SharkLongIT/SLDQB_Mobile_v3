using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Configuration.Dto
{
	public class CmsSettingsEditDto
	{
		public CmsGeneralSettingsEditDto General { get; set; }
		public CmsRobotsTxtSettingsEditDto RobotsTxt { get; set; }
	}
}
