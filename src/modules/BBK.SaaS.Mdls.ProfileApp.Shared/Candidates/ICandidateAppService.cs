using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
	public interface ICandidateAppService : IApplicationService, ITransientDependency
    {
		Task<GetCandidateForEditOutput> GetCandidateForEdit(NullableIdDto<long> input);
		Task<CandidateEditDto> Update(CandidateEditDto input);
		Task<bool> UpdateCandidateBL(NullableIdDto<long> input, string fileUrl);
		Task<long> UpdateMobile(CandidateEditDto input);

        Task GeneratePdf(long JobId,int TemplateId);
		Task UpdateProfilePictureFromMobile(byte[] imageBytes);


    }
}
