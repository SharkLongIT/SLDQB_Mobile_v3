using Abp.AutoMapper;
using BBK.SaaS.Mdls.Cms.Widgets.Dto;

namespace BBK.SaaS.Web.Areas.Cms.Models.Widgets
{
	[AutoMapFrom(typeof(GetConfigWidgetForEditOutput))]
	public class CreateOrEditConfigModalViewModel : GetConfigWidgetForEditOutput
	{
	}
}
