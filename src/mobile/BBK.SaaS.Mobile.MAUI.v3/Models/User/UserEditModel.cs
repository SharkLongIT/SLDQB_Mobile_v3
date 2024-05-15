using Abp.AutoMapper;
using BBK.SaaS.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(UserEditDto))]

    public class UserEditModel : UserEditDto
    {
    }
}
