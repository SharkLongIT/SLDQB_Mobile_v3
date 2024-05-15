using Abp.AutoMapper;
using BBK.SaaS.MultiTenancy.Dto;

namespace BBK.SaaS.Web.Models.TenantRegistration
{
    [AutoMapFrom(typeof(EditionsSelectOutput))]
    public class EditionsSelectViewModel : EditionsSelectOutput
    {
    }
}
