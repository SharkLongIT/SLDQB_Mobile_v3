using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Categories.Dto
{
  public class MoveCmsCatInput
  {
    [Range(1, long.MaxValue)]
    public long Id { get; set; }

    public long? NewParentId { get; set; }
  }

  public class SortCmsCatInput
  {
    [Range(1, long.MaxValue)]
    public long? ParentId { get; set; }

    public List<long> SortedIds { get; set; }
  }
}
