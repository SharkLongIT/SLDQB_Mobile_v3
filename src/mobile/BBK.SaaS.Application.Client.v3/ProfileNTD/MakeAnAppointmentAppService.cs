using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.ProfileNTD
{
    public partial class MakeAnAppointmentAppService : ProxyAppServiceBase, IMakeAnAppointmentAppService
    {
        public async Task<long> Create(MakeAnAppointmentDto input)
        {
            return await ApiClient.PostAsync<long>(GetEndpoint(nameof(Create)), input);  

        }

        public async Task<PagedResultDto<MakeAnAppointmentDto>> GetAll(MakeAnAppointmentInput input)
        {
           return await ApiClient.GetAsync<PagedResultDto<MakeAnAppointmentDto>>(GetEndpoint(nameof(GetAll)), input);
        }

        public async Task<PagedResultDto<MakeAnAppointmentDto>> GetAllOfCandidate(MakeAnAppointmentInput input)
        {
            return await ApiClient.GetAsync<PagedResultDto<MakeAnAppointmentDto>>(GetEndpoint(nameof(GetAllOfCandidate)), input);
        }

        public async Task<MakeAnAppointmentDto> GetDetail(long Id)
        {
            return await ApiClient.GetAsync<MakeAnAppointmentDto>(GetEndpoint(nameof(GetDetail)), new {Id = Id});
        }

        public async Task<MakeAnAppointmentDto> Update(MakeAnAppointmentDto input)
        {
            return await ApiClient.PutAsync<MakeAnAppointmentDto>(GetEndpoint(nameof(Update)), input);
        }

        public async Task<MakeAnAppointmentForUpdateMobile> UpdateAppForMobile(MakeAnAppointmentForUpdateMobile input)
        {
            return await ApiClient.PutAsync<MakeAnAppointmentForUpdateMobile>(GetEndpoint(nameof(UpdateAppForMobile)), input);
        }

        public async Task<MakeAnAppointmentDto> UpdateForCandidate(MakeAnAppointmentDto input)
        {
            return await ApiClient.PutAsync<MakeAnAppointmentDto>(GetEndpoint(nameof(UpdateForCandidate)), input);
        }
    }
}
