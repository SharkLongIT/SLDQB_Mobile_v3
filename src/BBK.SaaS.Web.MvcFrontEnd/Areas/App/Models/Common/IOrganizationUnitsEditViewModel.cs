using System.Collections.Generic;
using BBK.SaaS.Organizations.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Common
{
    public interface IOrganizationUnitsEditViewModel
    {
        List<OrganizationUnitDto> AllOrganizationUnits { get; set; }

        List<string> MemberedOrganizationUnits { get; set; }
    }
}