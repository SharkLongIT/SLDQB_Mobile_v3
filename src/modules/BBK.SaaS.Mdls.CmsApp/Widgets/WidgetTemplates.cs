using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;

namespace BBK.SaaS.Mdls.Cms.Widgets
{
	public class WidgetTemplate
	{
		public string Name { get; set; }
		public string Content { get; set; }
		public string PlaceHolderStr { get; set; }
		public List<WidgetField> Fields { get; set; }

	}

	public class WidgetField
	{
		public string Title { get; set; }
		public string Name { get; set; }
		public string Value { get; set; }
		public WidgetFieldTypeEnum FieldType { get; set; } = WidgetFieldTypeEnum.text;
	}

	public enum WidgetFieldTypeEnum : byte
	{
		text, img, video, link
	}
}
