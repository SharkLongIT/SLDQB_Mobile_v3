using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Entities
{
	/// <summary>
	/// Topic/Page
	/// </summary>
	[Table("AppTopics", Schema = SaaSCmsConsts.DefaultSchema)]
	public class Topic : FullAuditedEntity<long>, IMustHaveTenant
	{
		/// <summary>
		/// Gets or sets a value the tenant id
		/// </summary>
		public int TenantId { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }

		/// <summary>
		/// Make page is static mean this page could not be deleted
		/// </summary>
		public bool IsStatic { get; set; }

		/// <summary>
		/// Use to call page with zone layouts
		/// </summary>
		public int PageTemplateType { get; set; }

		[Required]
		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the title
		/// </summary>
		public string Title { get; set; }

		[MaxLength(SaaSConsts.MaxDescription)]
		/// <summary>
		/// Gets or sets the body
		/// </summary>
		public string Body { get; set; }

		[Required]
		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets/sets the unique slug.
		/// </summary>
		public string Slug { get; set; }

		[MaxLength(SaaSConsts.MaxShortDescLength)]
		/// <summary>
		/// Gets or sets the meta keywords
		/// </summary>
		public string MetaKeywords { get; set; }

		/// <summary>
		/// Gets or sets the meta description
		/// </summary>
		[MaxLength(SaaSConsts.MaxDescription)]
		public string MetaDescription { get; set; }

		/// <summary>
		/// Gets or sets the meta title
		/// </summary>
		[MaxLength(SaaSConsts.MaxShortDescLength)]
		public string MetaTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph title.
		/// </summary>
		[MaxLength(SaaSConsts.MaxShortDescLength)]
		public string OgTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph description.
		/// </summary>
		[MaxLength(SaaSConsts.MaxDescription)]
		public string OgDescription { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph image.
		/// </summary>
		[MaxLength(SaaSConsts.MaxUrlLength)]
		public string OgImageUrl { get; set; }

		/// <summary>
		/// Gets/sets the optional redirect.
		/// </summary>
		/// <returns></returns>
		//public string RedirectUrl { get; set; }

		/// <summary>
		/// Gets/sets the redirect type.
		/// </summary>
		/// <returns></returns>
		//public Models.RedirectType RedirectType { get; set; }

		/// <summary>
		/// Gets or sets the value indicating whether this topic is password protected
		/// </summary>
		public bool IsPasswordProtected { get; set; }

		/// <summary>
		/// Gets or sets the password
		/// </summary>
		[MaxLength(SaaSConsts.MaxCodeLineLength)]
		public string Password { get; set; }

		public long ViewedCount { get; set; }
	}
}
