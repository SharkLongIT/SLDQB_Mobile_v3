using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public interface INVNVCandidateAppService : IApplicationService, ITransientDependency
    {
        #region Service for Nhân viên nghiệp vụ
        Task<PagedResultDto<GetJobApplicationForEditOutput>> GetAllJobOfProfessionalStaff(JobAppSearchOfProfessionalStaff input);
        Task<PagedResultDto<GetCandidateForEditOutput>> GetAllCandidateOfProfessionalStaff(JobAppSearchOfProfessionalStaff input);

        #endregion
    }
}
