using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;
using BBK.SaaS.Mdls.Cms.Categories.MDto;

namespace BBK.SaaS.Mdls.Cms.Categories
{
	public interface IFECntCategoryAppService : IApplicationService
	{
		Task<ListResultDto<CmsCatDto>> GetMenuCategories();
		Task<ListResultDto<ContentCategoryDto>> GetCntCategoriesWithArticles();
		Task<ContentCategoryDto> GetCategory(GetCategoryInput input);
	}
}
