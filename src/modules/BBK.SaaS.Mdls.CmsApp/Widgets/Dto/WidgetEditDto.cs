using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp;

namespace BBK.SaaS.Mdls.Cms.Widgets.Dto
{
	public class WidgetEditDto : CreateWidgetInput
	{
		public int Id { get; set; }

		public virtual Guid UnqueId { get; set; }
	}

	public class CreateWidgetInput
	{
		public bool Published { get; set; }

		public int OrderIndex { get; set; } = 0;

		public string Title { get; set; }

		public string HTMLContent { get; set; }

		public string WidgetTemplateName { get; set; }

		public List<NameValue<string>> Fields { get; set; }
	}

	public class WidgetSelectDto
	{
		public int? Id { get; set; }

		public virtual Guid UnqueId { get; set; }

		public string Title { get; set; }

	}
}
