using Abp.AutoMapper;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Web.Areas.Profile.Models.Candidates;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Profile.Models.JobApplication
{
    [AutoMapFrom(typeof(GetJobApplicationForEditOutput))]
    public class JobApplicationModel : GetJobApplicationForEditOutput
    {
        public FileDto File { get; set; }
        public int CandidateAge { get; set; }
        public string Age { get; set; }
        public string LastModificationTimeString { get; set; }

        public JobAppPartialView JobAppPartialView { get; set; }
    }
    public class GetAllJobOfCandidate
    {
        public List<JobApplicationModel> Candidate { get; set; }
        public int Count;
        public bool IsCandidate { get; set; }
        public bool IsRecruiters { get; set; }

       
    }



    public class JobAppPartialView
    {

        public bool CanUpdate { get; set; } 
        public long? CandidateId { get; set; }   
        public long? Id { get; set; }
        public List<SelectListItem> Positions { get; set; } //Vị trí muốn ứng tuyển 
        public List<SelectListItem> Occupations { get; set; } //Nghề nghiệp
        public List<SelectListItem> WorkSite { get; set; } //Nơi muốn làm việc 
        public List<SelectListItem> Experiences { get; set; } //Kinh nghiệm làm việc
        public List<SelectListItem> Literacy { get; set; } //Trình độ học vấn
        public List<SelectListItem> FormOfWork { get; set; } //hình thức làm việc
    }

    public class WorkExpPartialView
    {

    }
    public class LearnProcessPartialView
    {

    }
}
