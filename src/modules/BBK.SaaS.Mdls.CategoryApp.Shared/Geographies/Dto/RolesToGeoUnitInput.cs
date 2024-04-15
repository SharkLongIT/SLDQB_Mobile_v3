using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class RolesToGeoUnitInput
    {
        public int[] RoleIds { get; set; }

        [Range(1, long.MaxValue)]
        public long GeoUnitId { get; set; }
    }
}