using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Security;
using System;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Profile.Models.Candidates
{
    [AutoMapFrom(typeof(GetCandidateForEditOutput))]

    public class CandidateDetailViewModel : GetCandidateForEditOutput
    {
        //public bool IsEditMode => User.Id.HasValue;

        public bool CanUpdateAvatar { get; set; } = false;
		public PasswordComplexitySetting PasswordComplexitySetting { get; set; }
       
	}


    [AutoMapFrom(typeof(JobAppSearch))]
    public class CandidateInput : JobAppSearch
    {
		public int PageSize { get; set; }
		public int Page { get; set; } = 1;
	}
}
