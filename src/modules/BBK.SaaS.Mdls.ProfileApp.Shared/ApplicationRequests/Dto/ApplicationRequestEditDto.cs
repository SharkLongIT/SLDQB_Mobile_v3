using Abp.Application.Services.Dto;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto
{
    public class ApplicationRequestEditDto  :  CreationAuditedEntityDto<long>
    {
        public int TenantId { get; set; }
        public long JobApplicationId { get; set; }
        public JobApplicationEditDto JobApplication { get; set; }
        public long RecruitmentId { get; set; }
        public RecruitmentDto Recruitment { get; set; }
        public UserEditDto User { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
        public Guid? ProfilePictureId { get; set; } 

    }
}
