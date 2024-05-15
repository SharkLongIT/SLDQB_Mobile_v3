using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;

namespace BBK.SaaS.Web.Models.Cms.Articles
{
	public class ArticleViewModel
	{
		//public ArticleViewDto Article { get; set; }
		public ArticleViewDto Article { get; set; }

		public ContentCategoryDto Category { get; set; }
	}
}
