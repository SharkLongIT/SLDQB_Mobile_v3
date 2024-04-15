using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class UserToGeoUnitInput
    {
        [Range(1, long.MaxValue)]
        public long UserId { get; set; }

        [Range(1, long.MaxValue)]
        public long GeoUnitId { get; set; }
    }
}