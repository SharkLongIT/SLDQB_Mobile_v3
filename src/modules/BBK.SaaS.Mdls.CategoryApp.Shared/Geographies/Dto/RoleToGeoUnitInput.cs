using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class RoleToGeoUnitInput
    {
        [Range(1, long.MaxValue)]
        public int RoleId { get; set; }

        [Range(1, long.MaxValue)]
        public long GeoUnitId { get; set; }
    }
}