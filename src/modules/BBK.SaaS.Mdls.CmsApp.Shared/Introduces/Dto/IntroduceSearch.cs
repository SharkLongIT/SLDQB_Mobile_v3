using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Introduces.Dto
{
    public class IntroduceSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public int? Status {  get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
