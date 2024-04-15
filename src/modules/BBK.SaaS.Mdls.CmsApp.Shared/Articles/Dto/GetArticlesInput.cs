using System;
using System.Collections.Generic;
using System.Text;
using Abp.Runtime.Validation;
using BBK.SaaS.Dto;

namespace BBK.SaaS.Mdls.Cms.Articles.Dto
{
	public class GetArticlesInput : PagedAndSortedInputDto, IShouldNormalize
	{
		public string Filter { get; set; }
		public DateTime? CreationDateStart { get; set; }
		public DateTime? CreationDateEnd { get; set; }

		public void Normalize()
		{
			//if (string.IsNullOrEmpty(Sorting))
			//{
			//	Sorting = "Title";
			//}

			//Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
			//{
			//    return s.Replace("editionDisplayName", "Edition.DisplayName");
			//});

			if (string.IsNullOrEmpty(Sorting))
			{
				Sorting = "CreationTime DESC";
			}
		}
	}
}
