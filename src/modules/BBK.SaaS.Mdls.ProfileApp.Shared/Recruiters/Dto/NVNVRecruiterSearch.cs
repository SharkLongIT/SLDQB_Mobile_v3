using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.Recruiters.Dto
{
    public class NVNVRecruiterSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public long? Address { get; set; }
        public long? SphereOfActivity { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
