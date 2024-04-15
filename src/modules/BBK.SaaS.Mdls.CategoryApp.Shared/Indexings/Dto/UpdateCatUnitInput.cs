using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Indexings.Dto
{
    public class UpdateCatUnitInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        [Required]
        [StringLength(CatUnit.MaxDisplayNameLength)]
        public string DisplayName { get; set; }
    }
}