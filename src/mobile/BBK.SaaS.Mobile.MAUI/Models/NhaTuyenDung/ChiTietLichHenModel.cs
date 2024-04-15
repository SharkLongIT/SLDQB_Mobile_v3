using BBK.SaaS.Mdls.Category.Indexings.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.NhaTuyenDung
{
    public class ChiTietLichHenModel
    {

        public long? Id { get; set; }
        public string Title { get; set; }
        public int TenantId { get; set; }

        public string TypeInterview { get; set; }

        public string Address { get; set; }

        public string Message { get; set; }

        public DateTime InterviewTime { get; set; }

        public long CandidateId { get; set; }

        public long JobApplicationId { get; set; }

        public long RecruiterId { get; set; }

        public long ApplicationRequestId { get; set; }

        public RecruiterEditDto Recruiter { get; set; }

        public JobApplicationEditDto JobApplication { get; set; }

        public CandidateEditDto Candidate { get; set; }

        public string Name { get; set; }

        public string Rank { get; set; }

        public CatUnitDto Ranks { get; set; }

        public long InterviewResultStatus { get; set; }

        public long InterviewResultLetter { get; set; }

        public RecruitmentDto Recruitment { get; set; }

        public long StatusOfCandidate { get; set; }

        public string ReasonForRefusal { get; set; }
    }
}
