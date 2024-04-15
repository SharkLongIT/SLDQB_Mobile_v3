using System;
using System.Collections.Generic;
using System.Text;
using Abp.Runtime.Validation;
using BBK.SaaS.Dto;

namespace BBK.SaaS.Mdls.Cms.Articles.MDto
{
	public class GetArticlesByCatInput : PagedAndSortedInputDto
	{
		public string Filter { get; set; }
		public long CategoryId { get; set; }
	}

	public class SearchArticlesInput : PagedAndSortedInputDto
	{
		public int? TenantId { get; set; }
		public string Filter { get; set; }
		public long? CategoryId { get; set; }
	}
}
