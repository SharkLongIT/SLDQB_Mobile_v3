using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.ViecTimNguoi
{
    public partial class RecruitmentAppService : ProxyAppServiceBase, IRecruitmentAppService
    {
        public async Task<PagedResultDto<RecruitmentDto>> GetAll(RecruimentInput input)
        {
            return await ApiClient.GetAsync<PagedResultDto<RecruitmentDto>>(GetEndpoint(nameof(GetAll)), input);
        }
        public async Task<RecruitmentDto> GetDetail(long Id)
        {
            return await ApiClient.GetAnonymousAsync<RecruitmentDto>(GetEndpoint(nameof(GetDetail)),  new{ Id = Id } );
        }
        //lay tat ca cac bai viet viec tim nguoi
        public async Task<PagedResultDto<RecruitmentDto>> GetAllByAllUser(RecruimentInput input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<RecruitmentDto>>(GetEndpoint(nameof(GetAllByAllUser)), input);
        }
        public async Task<PagedResultDto<RecruitmentDto>> GetAllUser(RecruimentInput input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<RecruitmentDto>>(GetEndpoint(nameof(GetAllUser)), input);
        }
        #region
        public async Task<long> Create(RecruitmentDto input)
        {
            return await ApiClient.PostAsync<long>(GetEndpoint(nameof(Create)), input);
        }

        public async Task DeleteDoc(long? Id)
        {
            await ApiClient.DeleteAsync(GetEndpoint(nameof(DeleteDoc)), new { Id = Id });
        }

        public async Task<long> Update(RecruitmentDto input)
        {
            return await ApiClient.PutAsync<long>(GetEndpoint(nameof(Update)), input);
        }

        public async Task<PagedResultDto<RecruitmentDto>> GetAllBy()
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<RecruitmentDto>>(GetEndpoint(nameof(GetAllBy)));
        }

        public Task<long> ActiveRecruiment(RecruitmentDto input)
        {
            throw new NotImplementedException();
        }

        public Task<PagedResultDto<RecruitmentDto>> GetAllByNVNV(RecruimentInput input)
        {
            throw new NotImplementedException();
        }

		public async Task<int> CountRecruiment()
		{
			return await ApiClient.PostAnonymousAsync<int>(GetEndpoint(nameof(CountRecruiment)));
		}



		#endregion

	}
}
