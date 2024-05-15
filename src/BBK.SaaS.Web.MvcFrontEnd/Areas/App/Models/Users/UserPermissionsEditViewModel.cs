using Abp.AutoMapper;
using BBK.SaaS.Authorization.Users;
using BBK.SaaS.Authorization.Users.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common;

namespace BBK.SaaS.Web.Areas.App.Models.Users
{
    [AutoMapFrom(typeof(GetUserPermissionsForEditOutput))]
    public class UserPermissionsEditViewModel : GetUserPermissionsForEditOutput, IPermissionsEditViewModel
    {
        public User User { get; set; }
    }
}