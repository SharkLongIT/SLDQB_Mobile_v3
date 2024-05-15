using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.TinTuc
{
    public class CmsCatsAppService : ProxyAppServiceBase, ICmsCatsAppService
    {
        public Task<ListResultDto<CmsCatDto>> GetCmsCats()
        {
            throw new NotImplementedException();
        }

        public async Task<ListResultDto<CmsCatDto>> GetCmsCatsByLevel(GetCmsCatInput input)
        {
            return await ApiClient.GetAnonymousAsync<ListResultDto<CmsCatDto>>(GetEndpoint(nameof(GetCmsCatsByLevel)), input);
            ;
        }

        public Task<ListResultDto<CmsCatDto>> GetCmsCatsByLevelAsync(GetCmsCatInput input)
        {
            throw new NotImplementedException();
        }
    }
}
