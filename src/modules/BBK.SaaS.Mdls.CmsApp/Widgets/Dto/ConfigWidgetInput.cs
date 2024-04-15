using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.Widgets.Dto
{
	public class CreateOrUpdateConfigWidgetInput
	{
		public string[] WidgetZoneName { get; set; }
		public string WidgetId { get; set; }
	}

	public class GetConfigWidgetForEditOutput
	{
		public WidgetSelectDto Widget { get; set; }
		public string[] WidgetZoneNames { get; set; }
		public List<string> DisplayedInZones { get; set; }

	}

	public class WidgetsToZoneInput
    {
        public int[] WidgetIds { get; set; }

        public string ZoneName { get; set; }
    }
}
