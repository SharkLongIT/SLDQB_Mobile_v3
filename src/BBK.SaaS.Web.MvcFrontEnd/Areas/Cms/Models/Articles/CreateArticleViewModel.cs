using System.Collections.Generic;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;

namespace BBK.SaaS.Web.Areas.Cms.Models.Articles
{
	public class CreateArticleViewModel
	{
		public long CategoryId { get; set; }
		public IReadOnlyList<CmsCatDto> Categories { get; set; }
		public CreateArticleViewModel()
		{
			Categories = new List<CmsCatDto>();
		}
	}

	public class EditArticleViewModel
	{
		public long CategoryId { get; set; }
		public IReadOnlyList<CmsCatDto> Categories { get; set; }

		public ArticleEditDto Article { get; set; }

		public EditArticleViewModel()
		{
			Categories = new List<CmsCatDto>();
		}
	}
}
