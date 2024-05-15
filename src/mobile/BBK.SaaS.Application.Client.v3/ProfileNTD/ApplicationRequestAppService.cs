using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.ApplicationRequests;
using BBK.SaaS.Mdls.Profile.ApplicationRequests.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.ProfileNTD
{
    public partial class ApplicationRequestAppService : ProxyAppServiceBase, IApplicationRequestAppService
    {
        public async Task<PagedResultDto<ApplicationRequestEditDto>> GetAllByRecruiter(ApplicationRequestSearch input)
        {
            return await ApiClient.GetAsync<PagedResultDto<ApplicationRequestEditDto>>(GetEndpoint(nameof(GetAllByRecruiter)), input);
        }
        public async Task<PagedResultDto<ApplicationRequestEditDto>> GetAll(ApplicationRequestSearch input)
        {
            return await ApiClient.GetAsync<PagedResultDto<ApplicationRequestEditDto>>(GetEndpoint(nameof(GetAll)), input);
        }
        public async Task<ApplicationRequestEditDto> GetById(NullableIdDto<long> dto)
        {
            return await ApiClient.GetAsync<ApplicationRequestEditDto>(GetEndpoint(nameof(GetById)), new {Id = dto.Id});
        }
        public Task<bool> CheckApplied(long RecruitmentId)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationRequestEditDto> Create(ApplicationRequestEditDto dto)
        {
            return await ApiClient.PostAsync<ApplicationRequestEditDto>(GetEndpoint(nameof(Create)), dto);
        }


        public async Task Delete(long? Id)
        {
            await ApiClient.DeleteAsync(GetEndpoint(nameof(Delete)), new {Id = Id});
        }

        public async Task DeleteApplicationRequest(NullableIdDto<long> dto)
        {
            await ApiClient.DeleteAsync(GetEndpoint(nameof(DeleteApplicationRequest)), new { Id = dto });
        }

        public Task<long> CreateUT(ApplicationRequestEditDto input)
        {
            throw new NotImplementedException();
        }
    }
}
