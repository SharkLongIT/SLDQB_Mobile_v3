using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	[Table("AppArticles", Schema = SaaSCmsConsts.DefaultSchema)]
	public class Article : FullAuditedEntity<long>, IMustHaveTenant
	{
		/// <summary>
		/// Gets or sets a value the tenant id
		/// </summary>
		public int TenantId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }

		[MaxLength(SaaSConsts.MaxSingleLineLength)]
		/// <summary>
		/// Gets or sets the title
		/// </summary>
		public string Title { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		public string ShortDesc { get; set; }

		[MaxLength(SaaSConsts.MaxContent)]
		/// <summary>
		/// Gets or sets the content
		/// </summary>
		public string Content { get; set; }

		[Required]
		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

		/// <summary>
		/// Gets/sets if comments should be enabled.
		/// </summary>
		/// <value></value>
		public bool AllowComments { get; set; }

		/// <summary>
		/// Gets or sets the news item start date and time
		/// </summary>
		public DateTime? StartDate { get; set; }

		/// <summary>
		/// Gets or sets the news item end date and time
		/// </summary>
		public DateTime? EndDate { get; set; }

		public long ViewedCount { get; set; }

		//public long CategoryId { get; set; }

		//[ForeignKey(nameof(CategoryId))]
		//public CmsCat Category { get; set; }

		[MaxLength(SaaSConsts.MaxUrlLength)]
		/// <summary>
		/// Gets/sets the optional open graph image.
		/// </summary>
		public string PrimaryImageUrl { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the meta keywords
		/// </summary>
		public string MetaKeywords { get; set; }

		[MaxLength(8000)]
		/// <summary>
		/// Gets or sets the meta description
		/// </summary>
		public string MetaDescription { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the meta title
		/// </summary>
		public string MetaTitle { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets/sets the optional open graph title.
		/// </summary>
		public string OgTitle { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets/sets the optional open graph description.
		/// </summary>
		public string OgDescription { get; set; }

		[MaxLength(SaaSConsts.MaxUrlLength)]
		/// <summary>
		/// Gets/sets the optional open graph image.
		/// </summary>
		public string OgImageUrl { get; set; }

		[MaxLength(SaaSConsts.MaxShortLineLength)]
		/// <summary>
		/// Gets/sets the author of Article.
		/// </summary>
		public string Author { get; set; }
	}
}
