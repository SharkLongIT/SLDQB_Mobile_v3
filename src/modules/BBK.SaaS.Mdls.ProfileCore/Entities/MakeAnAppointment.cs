using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using BBK.SaaS.Mdls.Category.Indexings;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BBK.SaaS.Mdls.Profile.Entities
{
    /// <summary>
    /// Kien : Bảng đặt lịch hẹn pv trên trang người tìm việc
    /// </summary>
    [Table("AppMakeAnAppointments", Schema = SaaSProfileConsts.DefaultSchema)]
    public class MakeAnAppointment : CreationAuditedEntity<long>, IMustHaveTenant, IHasCreationTime
    {
        public int TenantId { get; set; }

        public long ApplicationRequestId { get; set; } // Từ ApplicationRequest nào

        [ForeignKey("ApplicationRequestId")]
        public Recruitment Recruitment { get; set; }

        public string TypeInterview { get; set; } // hình thức phỏng vấn
        public string Address { get; set; } // địa chỉ phỏng vấn
        public string Message { get; set; } // tin nhắn
        public DateTime InterviewTime { get; set; } // thời gian phỏng vấn

        // JA ID
        public long JobApplicationId { get; set; } // id cv
        [ForeignKey("JobApplicationId")]
        public JobApplication JobApplication { get; set; }

        public long CandidateId { get; set; } // người lao động
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }

        public long RecruiterId { get; set; }  // nhà tuyển dụng
        [ForeignKey("RecruiterId")]
        public Recruiter Recruiter { get; set; }

        public long InterviewResultStatus { get; set; } // trạng thái // 1.đỗ 2. trượt 

        public long InterviewResultLetter { get; set; } // kết quả pv
        public long Rank { get; set; } // Cấp bậc
        [ForeignKey("Rank")]
        public CatUnit Ranks { get; set; }

        public long StatusOfCandidate { get; set; } // trạng thái của người lao động // 1.Chờ pv, 2.Xác nhận pv, 3.Từ chối pv, 4.Đỗ pv 

        public string ReasonForRefusal { get; set;} // lý do từ chối


        // InterviewResultStatus:Pass|Fail|Cancel, InterviewResultLetter:text
    }


}
