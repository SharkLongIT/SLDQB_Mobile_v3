using System;
using System.Collections.Generic;
using System.Text;
using Abp.Domain.Entities;
using BBK.SaaS.Storage;

namespace BBK.SaaS.Mdls.Cms.Articles.Dto
{
	public class ArticleEditDto : Entity<long>
	{
		/// <summary>
		/// Gets or sets a value indicating whether the entity is published
		/// </summary>
		public bool Published { get; set; }

		/// <summary>
		/// Gets or sets the title
		/// </summary>
		public string Title { get; set; }

		public string ShortDesc { get; set; }

		/// <summary>
		/// Gets or sets the body
		/// </summary>
		public string Content { get; set; }

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

		//public DateTime? CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the meta keywords
		/// </summary>
		public string MetaKeywords { get; set; }

		/// <summary>
		/// Gets or sets the meta description
		/// </summary>
		public string MetaDescription { get; set; }

		/// <summary>
		/// Gets or sets the meta title
		/// </summary>
		public string MetaTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph title.
		/// </summary>
		public string OgTitle { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph description.
		/// </summary>
		public string OgDescription { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph image.
		/// </summary>
		public string OgImageUrl { get; set; }

		/// <summary>
		/// Gets/sets the optional open graph image.
		/// </summary>
		public string PrimaryImageUrl { get; set; }

		public string PrimaryImageData { get; set; }

		public FileMgr PrimaryImage { get; set; }

		public string Author { get; set; }
		public long ViewedCount { get; set; }

		public long? CategoryId { get; set; }
	}
}
