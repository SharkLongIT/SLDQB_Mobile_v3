using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.TradingSessions
{
    public interface ITradingSessionAccountAppService :IApplicationService
    {
        Task<long> Create(TradingSessionAccountEditDto input);
        Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllRecuiter(TradingSessionAccountSeach input);
        Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllCandidate(TradingSessionAccountSeach input);
        Task<PagedResultDto<TradingSessionEditDto>> GetAll(TradingSessionSearch input);
        int CheckAccount(long? TradingSessionId);

        Task<PagedResultDto<RecruiterEditDto>> GetAllRecruiterNot(TradingSessionAccountSeach input);
        Task<PagedResultDto<TradingSessionAccountEditDto>> GetAllByUserId(TradingSessionSearch input);
        Task<PagedResultDto<JobApplicationEditDto>> GetAllCandidateNot(TradingSessionAccountSeach input);

        
        #region Cập nhật trạng thái từ chối, xác nhận tham gia phiên giao dịch
        Task UpdateStatus(TradingSessionAccountEditDto input);
        #endregion
        Task UpdateStatusByTradingId(TradingSessionAccountEditDto input);


        Task<ReportArray> GetByChart(long Id);

    }
}
