using Abp.AutoMapper;
using BBK.SaaS.Mdls.Category.Indexings;

namespace BBK.SaaS.Web.Areas.Lib.Models.Indexings
{
	[AutoMapFrom(typeof(CatUnit))]
	public class EditCategoryUnitModalViewModel
    {
		public long? Id { get; set; }

		public string DisplayName { get; set; }
	}
}