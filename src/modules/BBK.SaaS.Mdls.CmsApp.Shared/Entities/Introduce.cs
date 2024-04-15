using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Mdls.Cms;
using BBK.SaaS.Mdls.Cms.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppIntroduces", Schema = SaaSCmsConsts.DefaultSchema)]
    public class Introduce : FullAuditedEntity<long>
    {
        public string FullName { get; set; } // tên
        public string Email { get; set; } // email
        public string Phone { get; set; } // số điện thoại
        public string Description { get; set; } // câu hỏi
        public int Status { get; set; } // trạng thái
        public long? ArticleId { get; set; } // bai viet

        [ForeignKey("ArticleId")]
        public Article Article { get; set;}
    }
}
