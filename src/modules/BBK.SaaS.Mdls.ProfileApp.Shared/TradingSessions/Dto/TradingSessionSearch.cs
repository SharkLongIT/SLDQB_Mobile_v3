using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.TradingSessions.Dto
{
    public class TradingSessionSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public List<long> WorkSite { get; set; }

        public int? StatusOfTradingSession { get; set; }
        public int? Status { get; set; }

        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
    }
}
