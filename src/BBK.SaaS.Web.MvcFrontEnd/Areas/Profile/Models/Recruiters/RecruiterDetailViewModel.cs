using Abp.AutoMapper;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Entities;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using BBK.SaaS.Security;
using BBK.SaaS.Web.Areas.Profile.Models.JobApplication;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Profile.Models.Recruiters
{
    [AutoMapFrom(typeof(GetRecruiterForEditOutput))]

	public class RecruiterDetailViewModel: GetRecruiterForEditOutput
	{
		public bool IsEditMode => User.Id.HasValue;

		public PasswordComplexitySetting PasswordComplexitySetting { get; set; }
	}


    [AutoMapFrom(typeof(RecruimentInput))]
    public class RecruimentsInput : RecruimentInput
    {
        public int PageSize { get; set; }
        public int Page { get; set; } = 1;

        public long? RecruiterUserId { get; set; }
    }

    [AutoMapFrom(typeof(RecruitmentDto))]
    public class JobUserModel : RecruitmentDto
    {
        public bool IsAppllied { get; set; }
       public string CompanyName { get; set; }
    }

    public class GetAllJobOfUser
    {
        public List<JobUserModel> Recruiment { get; set; }
        public Recruiter Recruiter { get; set; }
        public int Count;


        // Cuong
        public bool IsCandidate { get; set; }   
        public bool IsRecuiters {  get; set; }

        public List<JobApplicationModel> Candidate { get; set; }
		public List<TradingSessionEditDto> TradingSession { get; set; }
		public List<ArticleListViewDto> Article { get; set; }
	}

    public class UpdateAvatarViewModel
    {
        public IFormFile Avatar { get; set; }
        public int Id { get; set; }
    }
}
