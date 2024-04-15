using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.UrlRecords.Dto
{
	public class GetUrlRecordsInput: PagedAndSortedInputDto, IShouldNormalize
	{
        public string Filter { get; set; }
        public DateTime? CreationDateStart { get; set; }
        public DateTime? CreationDateEnd { get; set; }

		public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Slug";
            }

            //Sorting = DtoSortingHelper.ReplaceSorting(Sorting, s =>
            //{
            //    return s.Replace("editionDisplayName", "Edition.DisplayName");
            //});
        }
	}
}
