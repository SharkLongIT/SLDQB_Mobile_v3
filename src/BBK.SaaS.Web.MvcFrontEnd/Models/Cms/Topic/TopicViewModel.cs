using Abp.AutoMapper;
using BBK.SaaS.Mdls.Cms.Topics.Dto;

namespace BBK.SaaS.Web.Models.Cms.Topic
{
	[AutoMapFrom(typeof(TopicEditDto))]
	public class TopicViewModel
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
