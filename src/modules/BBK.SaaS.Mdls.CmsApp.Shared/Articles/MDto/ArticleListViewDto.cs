using System;
using System.Collections.Generic;
using System.Text;
using BBK.SaaS.Mdls.Cms.Entities;

namespace BBK.SaaS.Mdls.Cms.Articles.MDto
{
    public class ArticleListViewDto
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public long? Id { get; set; }
        public string Title { get; set; }
        public string ShortDesc { get; set; }
        public string Author { get; set; }
        /// <summary>
        /// Gets/sets the unique slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the optional open graph image.
        /// </summary>
        public string PrimaryImageUrl { get; set; }

        public DateTime Modified {  get; set; }
        public ArticleListViewDto() { }

        public ArticleListViewDto(Article article)
        {
            this.Title = article.Title;
            this.ShortDesc = article.ShortDesc;
            this.Slug = article.Slug;
            this.PrimaryImageUrl = article.PrimaryImageUrl;
            this.Author = article.Author;
            this.Modified = article.CreationTime;
        }

    }
}
