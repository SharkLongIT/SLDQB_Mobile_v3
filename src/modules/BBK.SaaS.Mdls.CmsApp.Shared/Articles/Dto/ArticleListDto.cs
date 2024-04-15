using System;
using System.Collections.Generic;
using System.Text;
using Abp.Application.Services.Dto;

namespace BBK.SaaS.Mdls.Cms.Articles.Dto
{
	public class ArticleListDto : EntityDto<long>
	{
		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }

		/// <summary>
		/// Gets or sets the title
		/// </summary>
		public string Title { get; set; }

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

		public DateTime CreationTime { get; set; }
	}
}
