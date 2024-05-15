using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Profile.Candidates.Dto;
using BBK.SaaS.Mdls.Profile.Contacts;
using BBK.SaaS.Mdls.Profile.Contacts.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.LienHeHoiDap
{
    public partial class ContactAppService : ProxyAppServiceBase, IContactAppService
    {
        public async Task<long> Create(ContactDto input)
        {
            return await ApiClient.PostAnonymousAsync<long>(GetEndpoint(nameof(Create)), input);
        }

        public Task<PagedResultDto<ContactDto>> GetAll(ContactSearch input)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResultDto<ContactDto>> GetAllOfMe(ContactSearch input)
        {
            return await ApiClient.GetAsync<PagedResultDto<ContactDto>>(GetEndpoint(nameof(GetAllOfMe)), input);
        }

        public async Task<ContactDto> GetById(NullableIdDto<long> input)
        {
            return await ApiClient.GetAsync<ContactDto>(GetEndpoint(nameof(GetById)), input);
        }

        public void SendMail(ContactDto input)
        {
            throw new NotImplementedException();
        }
    }
}
