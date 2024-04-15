using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    [Table("AppContacts", Schema = SaaSProfileConsts.DefaultSchema)]

    public class Contact : AuditedEntity<long>
    {
        public string FullName { get; set; } // tên
        public string Email { get; set; } // email
        public string Phone { get; set; } // số điện thoại
        public string Description { get; set; } // câu hỏi
        public bool Status { get; set; } // trạng thái

        public string Answer { get; set; } // câu trả lời
    }
}
