using System.ComponentModel.DataAnnotations;

namespace BBK.SaaS.Mdls.Category.Indexings.Dto
{
    public class MoveCatUnitInput
    {
        [Range(1, long.MaxValue)]
        public long Id { get; set; }

        public long? NewParentId { get; set; }
    }
}