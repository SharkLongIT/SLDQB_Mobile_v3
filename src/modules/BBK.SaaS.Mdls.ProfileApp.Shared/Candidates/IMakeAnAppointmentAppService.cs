using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Candidates
{
    public interface IMakeAnAppointmentAppService : IApplicationService
    {
        Task<long> Create(MakeAnAppointmentDto input);//đặt lịch
        Task<MakeAnAppointmentDto> Update(MakeAnAppointmentDto input);
        Task<MakeAnAppointmentDto> UpdateForCandidate(MakeAnAppointmentDto input);
        Task<PagedResultDto<MakeAnAppointmentDto>> GetAll(MakeAnAppointmentInput input);
        Task<MakeAnAppointmentDto> GetDetail(long Id);
        Task<PagedResultDto<MakeAnAppointmentDto>> GetAllOfCandidate(MakeAnAppointmentInput input);

        #region Mobile
        Task<MakeAnAppointmentForUpdateMobile> UpdateAppForMobile(MakeAnAppointmentForUpdateMobile input);
        #endregion

    }
}
