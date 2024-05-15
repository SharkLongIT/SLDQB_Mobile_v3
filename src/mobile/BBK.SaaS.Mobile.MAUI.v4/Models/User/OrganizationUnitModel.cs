using Abp.AutoMapper;
using BBK.SaaS.Organizations.Dto;

namespace BBK.SaaS.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}
