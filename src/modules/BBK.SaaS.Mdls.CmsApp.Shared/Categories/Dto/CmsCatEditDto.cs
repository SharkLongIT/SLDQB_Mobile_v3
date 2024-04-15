using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Abp.Domain.Entities;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Categories.Dto
{
	public class CmsCatEditDto : Entity<long>
	{
		public Guid UnqueId { get; set; }

		/// <summary>
		/// Parent <see cref="CatUnit"/>.
		/// Null, if this OU is root.
		/// </summary>
		public CmsCatEditDto Parent { get; set; }

		/// <summary>
		/// Parent <see cref="CatUnit"/> Id.
		/// Null, if this OU is root.
		/// </summary>
		public long? ParentId { get; set; }

		/// <summary>
		/// Hierarchical Code of this organization unit.
		/// Example: "00001.00042.00005".
		/// This is a unique code for a Tenant.
		/// It's changeable if OU hierarch is changed.
		/// </summary>
		[StringLength(CmsCat.MaxCodeLength)]
		public  string Code { get; set; }

		/// <summary>
		/// Display name of this role.
		/// </summary>
		[StringLength(CmsCat.MaxDisplayNameLength)]
		public  string DisplayName { get; set; }

		[StringLength(SaaSConsts.MaxSingleLineLength)]
		public  string Slug {  get; set; }

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
