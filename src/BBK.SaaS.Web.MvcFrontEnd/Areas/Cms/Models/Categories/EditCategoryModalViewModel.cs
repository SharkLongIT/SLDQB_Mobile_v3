using System.ComponentModel.DataAnnotations;
using Abp.AutoMapper;
using Abp.Organizations;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Web.Areas.Cms.Models.Categories
{
	[AutoMapFrom(typeof(CmsCat))]
	public class EditCategoryModalViewModel
	{
		public long? ParentId { get; set; }

		public long Id { get; set; }

		public string DisplayName { get; set; }

		public string Slug { get; set; }

		[MaxLength(SaaSConsts.MaxShortLineLength)]
		/// <summary>
		/// Gets or sets the meta keywords
		/// </summary>
		public string MetaKeywords { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the meta description
		/// </summary>
		public string MetaDescription { get; set; }

		[MaxLength(SaaSConsts.MaxShortLineLength)]
		/// <summary>
		/// Gets or sets the meta title
		/// </summary>
		public string MetaTitle { get; set; }
	}
}