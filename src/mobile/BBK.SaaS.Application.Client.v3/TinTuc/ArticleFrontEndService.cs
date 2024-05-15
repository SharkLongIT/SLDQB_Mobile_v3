using Abp.Application.Services.Dto;
using BBK.SaaS.Authorization.Users.Profile.Dto;
using BBK.SaaS.Mdls.Cms.Articles;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.TinTuc
{
    public partial class ArticleFrontEndService : ProxyAppServiceBase, IArticleFrontEndService
    {
        public async Task<ArticleViewDto> GetArticleDetail(EntityDto<long> entityDto)
        {
            return await ApiClient.GetAnonymousAsync<ArticleViewDto>(GetEndpoint(nameof(GetArticleDetail)), new { Id = entityDto.Id });
        }

        public async Task<ListResultDto<ArticleListViewDto>> GetArticles(SearchArticlesInput input)
        {
            return await ApiClient.GetAnonymousAsync<ListResultDto<ArticleListViewDto>>(GetEndpoint(nameof(GetArticles)), input);
        }

        public async Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input)
        {
            return await ApiClient.GetAnonymousAsync<ListResultDto<ArticleListViewDto>>(GetEndpoint(nameof(GetArticlesByCategory)), input);
        }

        public async Task<ListResultDto<ArticleListViewDto>> GetNewestArticles()
        {
            return await ApiClient.GetAnonymousAsync<ListResultDto<ArticleListViewDto>>(GetEndpoint(nameof(GetNewestArticles)));

        }

        public async Task<string> GetPicture(string encryptedUrl)
        {
            return await ApiClient.GetAnonymousAsync<string>(GetEndpoint(nameof(GetPicture)), new { encryptedUrl = encryptedUrl });
        }
    }
}
