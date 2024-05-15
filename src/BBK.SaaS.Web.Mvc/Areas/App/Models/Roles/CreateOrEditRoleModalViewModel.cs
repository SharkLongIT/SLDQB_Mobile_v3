using Abp.AutoMapper;
using BBK.SaaS.Authorization.Roles.Dto;
using BBK.SaaS.Web.Areas.App.Models.Common;

namespace BBK.SaaS.Web.Areas.App.Models.Roles
{
    [AutoMapFrom(typeof(GetRoleForEditOutput))]
    public class CreateOrEditRoleModalViewModel : GetRoleForEditOutput, IPermissionsEditViewModel
    {
        public bool IsEditMode => Role.Id.HasValue;
    }
}