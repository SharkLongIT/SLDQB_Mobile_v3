using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Introduces;
using BBK.SaaS.Mdls.Cms.Introduces.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Introduce
{
    public partial class IntroduceAppService : ProxyAppServiceBase, IIntroduceAppService
    {

        public async Task<PagedResultDto<IntroduceEditDto>> GetAllByUserType(IntroduceSearch input)
        {
            return await ApiClient.GetAsync<PagedResultDto<IntroduceEditDto>>(GetEndpoint(nameof(GetAllByUserType)), input);
        }
        #region
        public async Task<long> Create(IntroduceEditDto input)
        {
            return await ApiClient.PostAsync<long>(GetEndpoint(nameof(Create)), input);
        }

        public Task Delete(long? Id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<IntroduceEditDto>> GetAll(IntroduceSearch input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<IntroduceEditDto>>(GetEndpoint(nameof(GetAll)), input);
        }



        public int GetCountByUserId()
        {
            throw new NotImplementedException();
        }

        public Task<long> Update(IntroduceEditDto input)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCountByUserIdForMobile()
        {
            return await ApiClient.GetAsync<int>(GetEndpoint(nameof(GetCountByUserIdForMobile)));
        }
        #endregion
    }
}
