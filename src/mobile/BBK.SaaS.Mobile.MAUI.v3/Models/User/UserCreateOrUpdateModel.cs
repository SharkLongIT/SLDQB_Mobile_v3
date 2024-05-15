using Abp.AutoMapper;
using BBK.SaaS.Authorization.Users.Dto;

namespace BBK.SaaS.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(CreateOrUpdateUserInput))]
    public class UserCreateOrUpdateModel : CreateOrUpdateUserInput
    {

    }
}
