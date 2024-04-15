using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
    public class CreateMediaFolderInput
    {
        public long? ParentId { get; set; }

        [Required]
        [StringLength(SaaSConsts.MaxShortLineLength)]
        public string DisplayName { get; set; } 
    }
}