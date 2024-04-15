using Abp.Runtime.Validation;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Category.Indexings;
using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;

namespace BBK.SaaS.Mdls.Profile.Candidates.Dto
{
    public class MakeAnAppointmentDto
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public string TypeInterview { get; set; } // hình thức phỏng vấn
        public string Address { get; set; } // địa chỉ phỏng vấn
        public string Message { get; set; } // tin nhắn
        public DateTime InterviewTime { get; set; } // thời gian phỏng vấn
        public long CandidateId { get; set; }
        public long JobApplicationId { get; set; }
        public long RecruiterId { get; set; }
        public long ApplicationRequestId { get; set; } // Từ ApplicationRequest nào
        public RecruiterEditDto Recruiter { get; set; }
        public JobApplicationEditDto JobApplication { get; set; }
        public CandidateEditDto Candidate { get; set; }
        public string Name { get; set; }
        public long Rank { get; set; } // Cấp bậc
        public CatUnitDto Ranks { get; set; } // Cấp bậc

        public long InterviewResultStatus { get; set; } // trạng thái 

        public long InterviewResultLetter { get; set; } // kết quả pv

        public RecruitmentDto Recruitment { get; set; }

        public long StatusOfCandidate { get; set; } // trạng thái của người lao động // 1.Chờ pv, 2.Xác nhận pv, 3.Từ chối pv, 4.Đỗ pv 

        public string ReasonForRefusal { get; set; } // lý do từ chối


    }

    public class MakeAnAppointmentForUpdateMobile
    {
        public long? Id { get; set; }
     
        public long JobApplicationId { get; set; }
  
        public long ApplicationRequestId { get; set; } // Từ ApplicationRequest nào

        public long InterviewResultLetter { get; set; } // kết quả pv

        public long StatusOfCandidate { get; set; } // trạng thái của người lao động // 1.Chờ pv, 2.Xác nhận pv, 3.Từ chối pv, 4.Đỗ pv 

        public string ReasonForRefusal { get; set; } // lý do từ chối

    }

    public class MakeAnAppointmentInput : PagedSortedAndFilteredInputDto, IShouldNormalize
    {
        public string Search { get; set; }
        public long Rank { get; set; }
        public long Experience { get; set; }
        public long InterviewResultLetter { get; set; } // kết quả pv
        public DateTime? InterviewTime { get; set; }
        public long? StatusOfCandidate { get; set; }
        public void Normalize()
        {
            if (string.IsNullOrEmpty(Sorting))
            {
                Sorting = "Id";
            }
        }
    }
}
