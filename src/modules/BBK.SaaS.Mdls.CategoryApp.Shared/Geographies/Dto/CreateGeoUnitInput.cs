using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Geographies.Dto
{
    public class CreateGeoUnitInput
    {
        public long? ParentId { get; set; }

        [Required]
        [StringLength(GeoUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; } 
    }
}