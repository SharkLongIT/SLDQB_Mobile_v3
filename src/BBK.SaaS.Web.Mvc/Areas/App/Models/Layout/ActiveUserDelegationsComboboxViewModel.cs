using System.Collections.Generic;
using BBK.SaaS.Authorization.Delegation;
using BBK.SaaS.Authorization.Users.Delegation.Dto;

namespace BBK.SaaS.Web.Areas.App.Models.Layout
{
    public class ActiveUserDelegationsComboboxViewModel
    {
        public IUserDelegationConfiguration UserDelegationConfiguration { get; set; }

        public List<UserDelegationDto> UserDelegations { get; set; }

        public string CssClass { get; set; }
    }
}
