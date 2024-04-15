using Abp.Application.Services.Dto;
using Abp.Runtime.Validation;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
	public class GetGeoUnitsInput : ISortedResultRequest, IShouldNormalize
	{
		public string Filter { get; set; }

		public string Sorting { get; set; }

		public long? ParentId { get; set; }

		public void Normalize()
		{
			if (string.IsNullOrEmpty(Sorting))
			{
				Sorting = "creationTime desc";
			}

			//Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
			//{
			//    if (s.Contains("displayName"))
			//    {
			//        s = s.Replace("displayName", "role.displayName");
			//    }

			//    if (s.Contains("addedTime"))
			//    {
			//        s = s.Replace("addedTime", "ouRole.creationTime");
			//    }

			//    return s;
			//});
		}
	}
}
