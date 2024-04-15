using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Articles.MDto;
using BBK.SaaS.Mdls.Cms.Categories.Dto;

namespace BBK.SaaS.Mdls.Cms.Categories
{
	public interface ICmsCatsAppService : IApplicationService
	{
		Task<ListResultDto<CmsCatDto>> GetCmsCats();
		Task<ListResultDto<CmsCatDto>> GetCmsCatsByLevel(GetCmsCatInput input);
	}
}
