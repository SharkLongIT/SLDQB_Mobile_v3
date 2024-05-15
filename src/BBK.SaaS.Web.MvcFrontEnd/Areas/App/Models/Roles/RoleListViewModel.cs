using System.Collections.Generic;
using Abp.Application.Services.Dto;
using BBK.SaaS.Authorization.Permissions.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common;

namespace BBK.SaaS.Web.Areas.App.Models.Roles
{
    public class RoleListViewModel : IPermissionsEditViewModel
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}