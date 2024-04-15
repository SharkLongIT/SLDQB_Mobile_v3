using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;

namespace BBK.SaaS.Mdls.Cms.Articles
{
	/// <summary>
	/// CMS Frontend Controller/Mobile
	/// </summary>
	public interface IArticleFrontEndService : IApplicationService
	{
		Task<ListResultDto<ArticleListViewDto>> GetArticles(SearchArticlesInput input);
		Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input);
		Task<ListResultDto<ArticleListViewDto>> GetNewestArticles();
		Task<ArticleViewDto> GetArticleDetail(EntityDto<long> entityDto);
		//Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input);
		Task<string> GetPicture(string encryptedUrl);
	}
}
