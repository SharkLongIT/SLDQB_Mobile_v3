using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.IO.Extensions;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using BBK.SaaS.Chat;
using BBK.SaaS.Storage;
using BBK.SaaS.Mdls.Cms.Widgets;

namespace BBK.SaaS.Web.Controllers
{
	public class CacheController : SaaSControllerBase
	{
		protected readonly IBinaryObjectManager BinaryObjectManager;
		protected readonly IChatMessageManager ChatMessageManager;
		private readonly IWidgetZoneManager WidgetZoneManager;

		public CacheController(IBinaryObjectManager binaryObjectManager, IChatMessageManager chatMessageManager, IWidgetZoneManager WidgetZoneManager)
		{
			BinaryObjectManager = binaryObjectManager;
			ChatMessageManager = chatMessageManager;
			this.WidgetZoneManager = WidgetZoneManager;
		}

		public async Task<JsonResult> ClearZone(string zoneName)
		{
			try
			{
				await WidgetZoneManager.ClearWidgetZoneCacheItemAsync(zoneName);
				//return Json(new { zoneName });

				return Json(new AjaxResponse(new
				{
					code = 200,
				}));
			}
			catch (UserFriendlyException ex)
			{
				return Json(new AjaxResponse(new ErrorInfo(ex.Message)));
			}
		}
	}
}