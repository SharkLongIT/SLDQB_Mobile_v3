using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using System.Threading.Tasks;

namespace BBK.SaaS.Mdls.Profile.Contacts
{
    public interface IContactAppService : IApplicationService
    {
        Task<PagedResultDto<ContactDto>> GetAll(ContactSearch input);
        Task<long> Create(ContactDto input);
        void SendMail(ContactDto input);
        Task<PagedResultDto<ContactDto>> GetAllOfMe(ContactSearch input);

        Task<ContactDto> GetById(NullableIdDto<long> input);
    }
}
