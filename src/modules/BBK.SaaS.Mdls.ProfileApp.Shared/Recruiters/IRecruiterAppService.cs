using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Recruiters
{
	public interface IRecruiterAppService : IApplicationService
    {
		Task<GetRecruiterForEditOutput> GetRecruiterForEdit(NullableIdDto<long> input);
		Task<bool> UpdateRecruiterBL(NullableIdDto<long> input, string fileUrl);
		Task<long> Create(RecruiterEditDto input);
		Task<long> Update(RecruiterEditDto input);
		Task<long> GetCrurrentUserId();
		Task<PagedResultDto<RecruiterEditDto>> GetAll();

        #region Mobile
        Task<long> UpdateRecruiterForMobile(RecruiterEditDto input);

        #endregion

    }
}
