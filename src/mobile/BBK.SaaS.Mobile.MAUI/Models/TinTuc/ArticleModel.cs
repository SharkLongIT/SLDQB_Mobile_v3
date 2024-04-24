using Abp.AutoMapper;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.TinTuc
{
    [AutoMapFrom(typeof(ArticleListViewDto))]

    public class ArticleModel : ArticleListViewDto
    {
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title { get; set; }
        public long? Id { get; set; }
        public long CategoryId { get; set; }
        public string ShortDesc { get; set; }
        public string Photo {  get; set; }
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
        public FileMgr PrimaryImage { get; set; }

        public string Author { get; set; }

        public DateTime LastModificationTime { get; set; }

        public long ViewedCount { get; set; }

    }
}
