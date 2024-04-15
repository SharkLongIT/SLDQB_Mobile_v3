using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Indexings.Dto
{
    public class CreateCatUnitInput
    {
        public long? ParentId { get; set; }

        [Required]
        [StringLength(SaaSConsts.MaxShortLineLength)]
        public string DisplayName { get; set; } 
    }
}