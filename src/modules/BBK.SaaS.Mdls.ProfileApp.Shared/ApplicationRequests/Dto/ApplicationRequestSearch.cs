using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto
{
  public class ApplicationRequestSearch : PagedSortedAndFilteredInputDto, IShouldNormalize
  {
    public string Search { get; set; }
    public int? Status { get; set; } 
    public long? Rank { get; set; } 
    public int? Experience { get; set; } 
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public void Normalize()
    {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "CreationTime DESC";
            }
        }
  }
}
