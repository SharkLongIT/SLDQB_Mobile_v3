using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace BBK.SaaS.Mdls.Profile.Contacts.Dto
{
    public class ContactDto : AuditedEntityDto<long>
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public string Answer { get; set; } // câu trả lời
    }


}
