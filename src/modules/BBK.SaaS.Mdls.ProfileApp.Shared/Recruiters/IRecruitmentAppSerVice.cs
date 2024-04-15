using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{

    public interface IRecruitmentAppService: IApplicationService

    {
        Task<PagedResultDto<RecruitmentDto>> GetAll(RecruimentInput input);
        Task<long> Create(RecruitmentDto input);
        Task<long> Update(RecruitmentDto input);
        Task DeleteDoc(long? Id);
        Task<RecruitmentDto> GetDetail(long Id);
        Task<PagedResultDto<RecruitmentDto>> GetAllByAllUser(RecruimentInput input);
        Task<PagedResultDto<RecruitmentDto>> GetAllBy();
        Task<long> ActiveRecruiment(RecruitmentDto input);
        Task<PagedResultDto<RecruitmentDto>> GetAllByNVNV(RecruimentInput input);
        #region Mobile/Frontend
        Task<PagedResultDto<RecruitmentDto>> GetAllUser(RecruimentInput input);
        #endregion
    }
}
