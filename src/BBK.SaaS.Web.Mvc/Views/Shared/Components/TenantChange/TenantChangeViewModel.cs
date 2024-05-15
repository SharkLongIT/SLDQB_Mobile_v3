using Abp.AutoMapper;
using BBK.SaaS.Sessions.Dto;

namespace BBK.SaaS.Web.Views.Shared.Components.TenantChange
{
    [AutoMapFrom(typeof(GetCurrentLoginInformationsOutput))]
    public class TenantChangeViewModel
    {
        public TenantLoginInfoDto Tenant { get; set; }
    }
}