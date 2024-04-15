using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Widgets.Dto
{
	public class WidgetListDto
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public bool Published { get; set; }
		public string Slug { get; set; }
		public bool IsPasswordProtected { get; set; }
		public DateTime CreationTime {  get; set; }
	}
}
