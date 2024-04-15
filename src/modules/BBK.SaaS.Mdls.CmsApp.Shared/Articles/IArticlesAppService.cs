using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;

namespace BBK.SaaS.Mdls.Cms.Articles
{
	public interface IArticlesAppService : IApplicationService
	{
		Task<ArticleEditDto> GetArticleForEditAsync(EntityDto<long> entityDto);
		//Task<ArticleViewDto> GetArticleDetailAsync(EntityDto<long> entityDto);

		Task<ListResultDto<ArticleListViewDto>> GetArticlesByCategory(GetArticlesByCatInput input);

		#region CMS Frontend Controller/Mobile
		Task<ListResultDto<ArticleListViewDto>> GetFEArticles(SearchArticlesInput input);
		Task<ListResultDto<ArticleListViewDto>> GetFENewestArticles();
		Task<ArticleViewDto> GetFEArticleDetail(EntityDto<long> entityDto);
		#endregion
	}
}
