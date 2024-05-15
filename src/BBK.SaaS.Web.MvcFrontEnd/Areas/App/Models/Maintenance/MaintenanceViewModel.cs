using System.Collections.Generic;
using BBK.SaaS.Caching.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Maintenance
{
    public class MaintenanceViewModel
    {
        public IReadOnlyList<CacheDto> Caches { get; set; }
        
        public bool CanClearAllCaches { get; set; }
    }
}