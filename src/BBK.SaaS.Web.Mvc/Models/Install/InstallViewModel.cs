using System.Collections.Generic;
using Abp.Localization;
using BBK.SaaS.Install.Dto;

namespace BBK.SaaS.Web.Models.Install
{
    public class InstallViewModel
    {
        public List<ApplicationLanguage> Languages { get; set; }

        public AppSettingsJsonDto AppSettingsJson { get; set; }
    }
}
