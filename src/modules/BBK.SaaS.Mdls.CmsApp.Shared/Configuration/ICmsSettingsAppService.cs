using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Abp.Application.Services;
using BBK.SaaS.Mdls.Cms.Configuration.Dto;

namespace BBK.SaaS.Mdls.Cms.Configuration
{
	public interface ICmsSettingsAppService : IApplicationService
	{
		Task<CmsSettingsEditDto> GetAllSettings();

		Task UpdateAllSettings(CmsSettingsEditDto input);
	}
}
