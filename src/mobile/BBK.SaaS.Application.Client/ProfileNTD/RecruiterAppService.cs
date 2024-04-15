using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.ProfileNTD
{
    public partial class RecruiterAppService : ProxyAppServiceBase, IRecruiterAppService
    {

        public Task<long> GetCrurrentUserId()
        {
            throw new NotImplementedException();
        }

        public async Task<GetRecruiterForEditOutput> GetRecruiterForEdit(NullableIdDto<long> input)
        {
            return await ApiClient.GetAnonymousAsync<GetRecruiterForEditOutput>(GetEndpoint(nameof(GetRecruiterForEdit)), input);
        }

        #region
        public Task<long> Create(RecruiterEditDto input)
        {
            throw new NotImplementedException();
        }


        public async Task<long> Update(RecruiterEditDto input)
        {
            return await ApiClient.PutAsync<long>(GetEndpoint(nameof(Update)), input);
        }

        public Task<bool> UpdateRecruiterBL(NullableIdDto<long> input, string fileUrl)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<RecruiterEditDto>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<long> UpdateRecruiterForMobile(RecruiterEditDto input)
        {
            return await ApiClient.PutAsync<long>(GetEndpoint(nameof(UpdateRecruiterForMobile)), input);
        }
        #endregion

    }
}
