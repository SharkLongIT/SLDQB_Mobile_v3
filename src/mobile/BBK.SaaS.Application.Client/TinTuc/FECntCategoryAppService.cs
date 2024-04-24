using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Categories;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.TinTuc
{
    public partial class FECntCategoryAppService : ProxyAppServiceBase, IFECntCategoryAppService
    {
        public async Task<ContentCategoryDto> GetCategory(GetCategoryInput input)
        {
            return await ApiClient.GetAnonymousAsync<ContentCategoryDto>(GetEndpoint(nameof(GetCategory)), input);

        }

        public Task<ListResultDto<ContentCategoryDto>> GetCntCategoriesWithArticles()
        {
            throw new NotImplementedException();
        }

        public Task<ListResultDto<CmsCatDto>> GetMenuCategories()
        {
            throw new NotImplementedException();
        }
    }
}
