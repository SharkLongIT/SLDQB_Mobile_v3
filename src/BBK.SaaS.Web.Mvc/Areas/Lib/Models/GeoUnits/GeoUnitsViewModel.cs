using Abp.Localization;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Lib.Models.GeoUnits
{
    public class GeoUnitsViewModel
    {
        public List<LanguageInfo> Languages { get; set; }
		public string BaseLanguageName { get; set; }
	}
}
