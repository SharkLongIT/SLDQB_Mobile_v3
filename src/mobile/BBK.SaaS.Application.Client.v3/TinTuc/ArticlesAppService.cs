using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Profile.Recruiters.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.TinTuc
{
    public partial class ArticlesAppService : ProxyAppServiceBase, IArticlesAppService
    {
     

     

        public Task<ArticleEditDto> GetArticleForEditAsync(EntityDto<long> entityDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<ArticleListViewDto>>(GetEndpoint(nameof(GetArticlesByCategory)), input);
        }

        public async Task<ArticleViewDto> GetFEArticleDetail(EntityDto<long> entityDto)
        {
            return await ApiClient.GetAnonymousAsync<ArticleViewDto>(GetEndpoint(nameof(GetFEArticleDetail)), new { Id = entityDto.Id });
        }

        public async Task<ListResultDto<ArticleListViewDto>> GetFEArticles(SearchArticlesInput input)
        {
            return await ApiClient.GetAnonymousAsync<PagedResultDto<ArticleListViewDto>>(GetEndpoint(nameof(GetFEArticles)), input);
        }

        public Task<ListResultDto<ArticleListViewDto>> GetFENewestArticles()
        {
            throw new NotImplementedException();
        }
    }
}
