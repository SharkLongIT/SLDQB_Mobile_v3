using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace BBK.SaaS.Mdls.Cms.Introduces.Dto
{
    public class IntroduceEditDto : FullAuditedEntityDto<long>
    {
        public string FullName { get; set; } // tên
        public string Email { get; set; } // email
        public string Phone { get; set; } // số điện thoại
        public string Description { get; set; } // câu hỏi
        public int Status { get; set; } // trạng thái
        public long? ArticleId { get; set; } // bai viet
        public Article Article { get; set; }

        public string Name { get; set; } // ten nguoi gioi thieu

    }
}
