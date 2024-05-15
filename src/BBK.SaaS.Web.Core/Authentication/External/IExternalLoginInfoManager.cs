using System.Collections.Generic;
using System.Security.Claims;
using BBK.SaaS.Web.Authentication.External;
using Abp.Dependency;
using Microsoft.AspNetCore.Identity;

namespace BBK.SaaS.Web.Authentication.External
{
    public interface IExternalLoginInfoManager : ITransientDependency
    {
        string GetUserNameFromClaims(List<Claim> claims);

        string GetUserNameFromExternalAuthUserInfo(ExternalAuthUserInfo userInfo);

        (string name, string surname) GetNameAndSurnameFromClaims(List<Claim> claims, IdentityOptions identityOptions);
    }
}
