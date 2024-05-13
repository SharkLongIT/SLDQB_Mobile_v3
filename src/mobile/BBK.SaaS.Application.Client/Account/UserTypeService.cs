using BBK.SaaS.Authorization.Accounts;
using BBK.SaaS.Authorization.Accounts.Dto;
using BBK.SaaS.Authorization.Users.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BBK.SaaS.Account
{
    public partial class UserTypeService : ProxyAppServiceBase, IUserTypeAppService
    {
        public async Task Register(int tenantId, UserEditDto input)
        {
            await ApiClient.PostAnonymousAsync(GetEndpoint(nameof(Register)),new { tenantId = 1, input});
        }

        public Task Update(int tenantId, UserEditDto input)
        {
            throw new NotImplementedException();
        }
    }
}
