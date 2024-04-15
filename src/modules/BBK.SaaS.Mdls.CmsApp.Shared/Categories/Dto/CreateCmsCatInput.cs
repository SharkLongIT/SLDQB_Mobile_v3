using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Categories.Dto
{
	public class CreateCmsCatInput
	{
		public long? ParentId { get; set; }

        [Required]
        [StringLength(SaaSConsts.MaxShortLineLength)]
        public string DisplayName { get; set; }

        [StringLength(SaaSConsts.MaxSingleLineLength)]
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
