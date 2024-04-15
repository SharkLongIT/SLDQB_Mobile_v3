using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;

namespace BBK.SaaS.Mdls.Profile.TradingSessions.Dto
{
    public class TradingSessionAccountEditDto
    {
        public long? Id { get; set; }
        public int TenantId { get; set; }
        public long UsertId { get; set; }

        public long? RecruiterId { get; set; }
        public RecruiterEditDto Recruiter { get; set; }

        public long? CandidateId { get; set; }
        public CandidateEditDto Candidate { get; set; }

        public long TradingSessionId { get; set; }

        public TradingSessionEditDto TradingSession { get; set; }

        public int Status { get; set; } = 1; // 0: Được mời, 1: Tham gia, 2:Từ chối



        // info candidate
        public string Positions { get; set; } // vi tri mong muon
        public string WorkSite { get; set; } // noi muon lam
        public string Occupations { get; set; } // nhom nganh nghe
        public string Experiences { get; set; } // kinh nghiem
        public string Literacy { get; set; } // hoc van
        public long WorkSiteId { get; set; } // id noi muon lam
        public long? JobApplicationId {  get; set; } // id ho so public
        public JobApplicationEditDto JobApplication { get; set; }   
        public Guid? ProfilePictureId { get; set; }
        public decimal? DesiredSalary { get; set; }

        public int count { get; set; }
        public bool IsCandidate { get; set; }
        public bool IsRecruiter { get; set; }

        // ngày tham gia
        public DateTime JoiningDate {  get; set; }  
        public int StatusOfTradingSession { get; set; } // 1:Sắp diễn ra , 2:Đang diễn ra, 3:Đã diễn ra

        // tin tuyen dung 
        public List<RecruitmentDto> Recruitment { get; set; }

    }



    public class ReportList
    {
        public virtual string NameRecruitment { get; set; }

        public virtual int CountRecruiment { get; set; }
        public virtual int CountJob { get; set; }
    }

    public class ReportArray
    {
        public virtual List<ReportList> ListReport { get; set; }
        public ReportArray()
        {
            ListReport = new List<ReportList>();
        }
    }

}
