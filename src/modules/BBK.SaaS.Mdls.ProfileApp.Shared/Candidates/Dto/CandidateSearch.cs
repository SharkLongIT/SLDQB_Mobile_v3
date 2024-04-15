using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.Candidates.Dto
{
    public class CandidateSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search {  get; set; }

        public int Take { get; set; }
        public DateTime? DateUpdate { get; set; }
        public List<long?> FormOfWork { get; set; }
        public long? Address { get; set; }
        public long? Literacy { get; set; }
        public int? DesiredSalary { get; set; }
       // public  Gender { get; set; }
        public long? Experience { get; set; }

        public int Paging { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
