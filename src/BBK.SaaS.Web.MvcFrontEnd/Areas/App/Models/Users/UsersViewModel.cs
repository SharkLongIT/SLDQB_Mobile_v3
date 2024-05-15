using System.Collections.Generic;
using Abp.Application.Services.Dto;
using BBK.SaaS.Authorization.Permissions.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common;

namespace BBK.SaaS.Web.Areas.App.Models.Users
{
    public class UsersViewModel : IPermissionsEditViewModel
    {
        public string FilterText { get; set; }

        public List<ComboboxItemDto> Roles { get; set; }

        public bool OnlyLockedUsers { get; set; }

        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}
