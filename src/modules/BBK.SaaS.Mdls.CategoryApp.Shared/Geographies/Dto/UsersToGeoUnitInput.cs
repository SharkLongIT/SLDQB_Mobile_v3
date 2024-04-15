using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class UsersToGeoUnitInput
    {
        public long[] UserIds { get; set; }

        [Range(1, long.MaxValue)]
        public long GeoUnitId { get; set; }
    }
}