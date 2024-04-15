using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using BBK.SaaS.Mdls.Cms.Topics.Dto;

namespace BBK.SaaS.Mdls.Cms.Topics
{
	public interface ITopicAppService : IApplicationService
	{
		Task<TopicEditDto> GetTopicForEditAsync(long id);
		Task<TopicEditDto> GetTopicDetailAsync(long id);
	}
}
