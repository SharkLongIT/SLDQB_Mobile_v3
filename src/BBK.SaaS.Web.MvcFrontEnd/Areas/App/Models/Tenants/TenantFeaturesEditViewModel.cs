using Abp.AutoMapper;
using BBK.SaaS.MultiTenancy;
using BBK.SaaS.MultiTenancy.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common;

namespace BBK.SaaS.Web.Areas.App.Models.Tenants
{
    [AutoMapFrom(typeof (GetTenantFeaturesEditOutput))]
    public class TenantFeaturesEditViewModel : GetTenantFeaturesEditOutput, IFeatureEditViewModel
    {
        public Tenant Tenant { get; set; }
    }
}