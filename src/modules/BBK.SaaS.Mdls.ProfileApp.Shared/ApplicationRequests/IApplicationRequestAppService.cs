using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Dependency;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.ApplicationRequests
{
    public interface IApplicationRequestAppService  : IApplicationService, ITransientDependency
    {
        Task<ApplicationRequestEditDto>  Create(ApplicationRequestEditDto dto);
        Task DeleteApplicationRequest(NullableIdDto<long> dto);
        Task<ApplicationRequestEditDto>  GetById(NullableIdDto<long> dto);
        Task<PagedResultDto<ApplicationRequestEditDto>> GetAll(ApplicationRequestSearch input);
        /// <summary>
        /// Kiểm tra người lao động đã ứng tuyển vào công việc recrumentId 
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckApplied(long RecruitmentId);
        Task<PagedResultDto<ApplicationRequestEditDto>> GetAllByRecruiter(ApplicationRequestSearch input);
        Task Delete(long? Id);                                                                                                                                                                                                                                                                                                                  
    }
}
                                                                                                                                                                       