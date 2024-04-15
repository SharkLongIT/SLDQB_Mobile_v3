using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class UpdateGeoUnitInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required]
        [StringLength(GeoUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}