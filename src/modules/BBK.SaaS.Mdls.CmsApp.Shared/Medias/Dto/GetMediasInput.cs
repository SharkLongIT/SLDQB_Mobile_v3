using System;
using System.Collections.Generic;
using System.Text;
using Abp.Runtime.Validation;
using BBK.SaaS.Dto;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
	public class GetMediasInput : PagedAndSortedInputDto, IShouldNormalize
	{
		//[Range(1, long.MaxValue)]
		public int? FolderId { get; set; }

		public string Filter { get; set; }

		public void Normalize()
		{
			if (string.IsNullOrEmpty(Sorting))
			{
				Sorting = "creationTime";
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
