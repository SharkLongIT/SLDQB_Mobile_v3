using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.TradingSessions.Dto
{
    public class TradingSessionAccountSeach : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public long? Id { get; set; }
        public List<long> WorkSite {  get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
