using System.Threading.Tasks;

namespace BBK.SaaS.Web.Authentication.External
{
    public interface IExternalAuthProviderApi
    {
        // Token: 0x17000014 RID: 20
        // (get) Token: 0x0600003C RID: 60
        ExternalLoginProviderInfo ProviderInfo { get; }

        // Token: 0x0600003D RID: 61
        Task<bool> IsValidUser(string userId, string accessCode);

        // Token: 0x0600003E RID: 62
        Task<ExternalAuthUserInfo> GetUserInfo(string accessCode);

        // Token: 0x0600003F RID: 63
        void Initialize(ExternalLoginProviderInfo providerInfo);
    }
}
