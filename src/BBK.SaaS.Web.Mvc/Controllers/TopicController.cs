using BBK.SaaS.Mdls.Cms.Topics;
using BBK.SaaS.Web.Models.Cms.Topic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Controllers
{
	[AutoValidateAntiforgeryToken]
	public class TopicController : SaaSControllerBase
	{
		private readonly ITopicAppService _topicAppService;

		public TopicController(ITopicAppService topicAppService)
		{
			_topicAppService = topicAppService;
		}

		[Route("Topic")]
		public async Task<IActionResult> Index(string id)
		{
			var dto = await _topicAppService.GetTopicDetailAsync(long.Parse(id));
			TopicViewModel model = ObjectMapper.Map<TopicViewModel>(dto);

			return View(model);
		}
	}
}
