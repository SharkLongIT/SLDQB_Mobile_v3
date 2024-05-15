using System.Collections.Generic;
using BBK.SaaS.Mdls.Cms.Categories.Dto;

namespace BBK.SaaS.Web.Areas.Cms.Models.Categories
{
	public class CreateCategoryModalViewModel
	{
		public long? ParentId { get; set; }

		public IReadOnlyList<CmsCatDto> Categories { get; set; } = new List<CmsCatDto>();

		public CreateCategoryModalViewModel(long? parentId)
		{
			ParentId = parentId;
		}
	}
}