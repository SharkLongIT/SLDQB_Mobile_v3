using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Cms.Introduces
{
    public interface IIntroduceAppService : IApplicationService
    {
        Task<PagedResultDto<IntroduceEditDto>> GetAll(IntroduceSearch input);
        Task<long> Create(IntroduceEditDto input);
        Task<long> Update(IntroduceEditDto input);
        Task Delete(long? Id);
        Task<int> GetCountByUserId();
        Task<PagedResultDto<IntroduceEditDto>> GetAllByUserType(IntroduceSearch input);


        #region Mobile

        Task<int> GetCountByUserIdForMobile();
        #endregion
    }
}
