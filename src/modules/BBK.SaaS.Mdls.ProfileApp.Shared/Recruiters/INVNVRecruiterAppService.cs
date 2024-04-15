using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{
    public interface INVNVRecruiterAppService : IApplicationService
    {
        Task<PagedResultDto<RecruiterEditDto>> GetAllRecruiter(NVNVRecruiterSearch input);
        Task Delete(long? Id);
        Task<long> Update(RecruiterEditDto input);
    }
}
