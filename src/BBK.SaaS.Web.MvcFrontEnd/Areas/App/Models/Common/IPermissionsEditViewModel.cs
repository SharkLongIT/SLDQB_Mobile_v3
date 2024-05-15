using System.Collections.Generic;
using BBK.SaaS.Authorization.Permissions.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }

        List<string> GrantedPermissionNames { get; set; }
    }
}