using Abp.Application.Services;
using BBK.SaaS.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public interface IReportJobApplicaiton : IApplicationService
    {
         Task<FileDto> ExportJobApplication(JobApplicationEditDto input); 
    }
}
