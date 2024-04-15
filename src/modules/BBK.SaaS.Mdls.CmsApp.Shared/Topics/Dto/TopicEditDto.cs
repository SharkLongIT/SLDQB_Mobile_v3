using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Topics.Dto
{
	public class TopicEditDto
	{
        public long? Id { get; set; }

		public bool Published { get; set; }

		public string Title { get; set; }

		public string Body { get; set; }

		public string Slug { get; set; }

		public string MetaKeywords { get; set; }

		public string MetaDescription { get; set; }

		public string MetaTitle { get; set; }

		public string OgTitle { get; set; }

		public string OgDescription { get; set; }

		public string OgImageUrl { get; set; }

		//public string RedirectUrl { get; set; }

		//public Models.RedirectType RedirectType { get; set; }

		public bool IsPasswordProtected { get; set; }

		public string Password { get; set; }
	}
}
