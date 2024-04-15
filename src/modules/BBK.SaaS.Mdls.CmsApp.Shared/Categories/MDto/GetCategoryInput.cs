using System;
using System.Collections.Generic;
using System.Text;
using BBK.SaaS.Mdls.Cms.Articles.MDto;

namespace BBK.SaaS.Mdls.Cms.Categories.MDto
{
	public class GetCategoryInput
	{
		public long CategoryId { get; set; }

		public SearchArticlesInput SearchArticlesInput { get; set; }
	}
}
