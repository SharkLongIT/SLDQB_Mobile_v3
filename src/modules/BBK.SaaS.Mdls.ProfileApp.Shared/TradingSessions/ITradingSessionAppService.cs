using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.TradingSessions.Dto;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.TradingSessions
{
    public interface ITradingSessionAppService : IApplicationService
    {
        Task<PagedResultDto<TradingSessionEditDto>> GetAll(TradingSessionSearch input);// tat ca
        Task<PagedResultDto<TradingSessionEditDto>> GetAllFuture(TradingSessionSearch input); // sắp diễn ra
        Task<PagedResultDto<TradingSessionEditDto>> GetAllPresent(TradingSessionSearch input); // đang diễn ra
        Task<PagedResultDto<TradingSessionEditDto>> GetAllPast(TradingSessionSearch input); // đã diễn ra
        Task<long> Create(TradingSessionEditDto input);
        Task<long> Update(TradingSessionEditDto input);
        Task Delete(long? Id);

        Task<TradingSessionEditDto> GetById(NullableIdDto<long> input);
        Task<PagedResultDto<TradingSessionEditDto>> GetAllUnitOfWork();
    }
}
