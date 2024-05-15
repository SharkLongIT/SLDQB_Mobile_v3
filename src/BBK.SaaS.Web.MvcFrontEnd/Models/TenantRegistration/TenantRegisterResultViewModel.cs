using Abp.AutoMapper;
using BBK.SaaS.MultiTenancy.Dto;

namespace BBK.SaaS.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(RegisterTenantOutput))]
    public class TenantRegisterResultViewModel : RegisterTenantOutput
    {
        public string TenantLoginAddress { get; set; }
    }
}