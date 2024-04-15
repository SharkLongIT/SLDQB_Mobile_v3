using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Cms.Medias.Dto
{
    public class MoveMediaFolderInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        public long? NewParentId { get; set; }
    }
}