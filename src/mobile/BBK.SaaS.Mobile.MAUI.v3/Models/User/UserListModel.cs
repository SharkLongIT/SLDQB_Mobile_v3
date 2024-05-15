using Abp.AutoMapper;
using BBK.SaaS.Authorization.Users.Dto;

namespace BBK.SaaS.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(UserListDto))]
    public class UserListModel : UserListDto
    {
        public string Photo { get; set; }

        public string FullName => Name + " " + Surname;
    }
}
