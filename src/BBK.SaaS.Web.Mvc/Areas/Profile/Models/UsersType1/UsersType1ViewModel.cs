using Abp.AutoMapper;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BBK.SaaS.Web.Areas.Profile.Models.UsersType1
{
	[AutoMapFrom(typeof(GetCandidateForEditOutput))]
	public class UsersType1ViewModel : GetCandidateForEditOutput 
    {
        public bool CanUpdateAvatar { get; set; } = false;
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

		public JoApplicationOfUsersType1ViewModel JobApplication {  get; set; }	

	}
	[AutoMapFrom(typeof(GetJobApplicationForEditOutput))]
	public class JoApplicationOfUsersType1ViewModel : GetJobApplicationForEditOutput
    {
		public int CandidateAge { get; set; }

		public JobAppPartialView JobAppPartialView { get; set; }
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

}
