using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using System;
using System.Threading.Tasks;

namespace BBK.SaaS.NguoiTimViec
{
    public class CandidateAppService : ProxyAppServiceBase, ICandidateAppService
    {

        public Task GeneratePdf(long JobId, int TemplateId)
        {
            throw new NotImplementedException();
        }

        public async Task<GetCandidateForEditOutput> GetCandidateForEdit(NullableIdDto<long> input)
        {
            return await ApiClient.GetAsync<GetCandidateForEditOutput>(GetEndpoint(nameof(GetCandidateForEdit)), input);
        }

        public async Task<CandidateEditDto> Update(CandidateEditDto input)
        {
            return await ApiClient.PostAsync<CandidateEditDto>(GetEndpoint(nameof(Update)), input);
        }

        public Task<bool> UpdateCandidateBL(NullableIdDto<long> input, string fileUrl)
        {
            throw new NotImplementedException();
        }

        public async Task<long> UpdateMobile(CandidateEditDto input)
        {
            return await ApiClient.PutAsync<long>(GetEndpoint(nameof(UpdateMobile)), input);
        }

        public async Task UpdateProfilePictureFromMobile(byte[] imageBytes)
        {
            await ApiClient.PutAsync(GetEndpoint(nameof(UpdateProfilePictureFromMobile)), imageBytes);
        }

    }
}
