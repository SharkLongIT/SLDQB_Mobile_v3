using System.Collections.Generic;
using BBK.SaaS.Web.Authentication.External;
using Abp.AutoMapper;

namespace BBK.SaaS.Web.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }

        public Dictionary<string, string> AdditionalParams { get; set; }

    }
}
