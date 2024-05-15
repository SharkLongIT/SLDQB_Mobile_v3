using Abp.Dependency;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BBK.SaaS.Web.Authentication.External
{
    public class ExternalAuthManager : IExternalAuthManager, ITransientDependency
    {
        // Token: 0x06000013 RID: 19 RVA: 0x000021A5 File Offset: 0x000003A5
        public ExternalAuthManager(IIocResolver iocResolver, IExternalAuthConfiguration externalAuthConfiguration)
        {
            this._iocResolver = iocResolver;
            this._externalAuthConfiguration = externalAuthConfiguration;
        }

        // Token: 0x06000014 RID: 20 RVA: 0x000021C0 File Offset: 0x000003C0
        public Task<bool> IsValidUser(string provider, string providerKey, string providerAccessCode)
        {
            Task<bool> result;
            using (IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> disposableDependencyObjectWrapper = this.CreateProviderApi(provider))
            {
                result = disposableDependencyObjectWrapper.Object.IsValidUser(providerKey, providerAccessCode);
            }
            return result;
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00002204 File Offset: 0x00000404
        public Task<ExternalAuthUserInfo> GetUserInfo(string provider, string accessCode)
        {
            Task<ExternalAuthUserInfo> userInfo;
            using (IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> disposableDependencyObjectWrapper = this.CreateProviderApi(provider))
            {
                userInfo = disposableDependencyObjectWrapper.Object.GetUserInfo(accessCode);
            }
            return userInfo;
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00002248 File Offset: 0x00000448
        public IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> CreateProviderApi(string provider)
        {
            bool flag = this._externalAuthConfiguration.ExternalLoginInfoProviders.Any((IExternalLoginInfoProvider infoProvider) => infoProvider.Name == provider);
            ExternalLoginProviderInfo externalLoginProviderInfo;
            if (flag)
            {
                externalLoginProviderInfo = this._externalAuthConfiguration.ExternalLoginInfoProviders.Single((IExternalLoginInfoProvider infoProvider) => infoProvider.Name == provider).GetExternalLoginInfo();
            }
            else
            {
                externalLoginProviderInfo = this._externalAuthConfiguration.Providers.FirstOrDefault((ExternalLoginProviderInfo p) => p.Name == provider);
            }
            bool flag2 = externalLoginProviderInfo == null;
            if (flag2)
            {
                throw new Exception("Unknown external auth provider: " + provider);
            }
            IDisposableDependencyObjectWrapper<IExternalAuthProviderApi> disposableDependencyObjectWrapper = IocResolverExtensions.ResolveAsDisposable<IExternalAuthProviderApi>(this._iocResolver, externalLoginProviderInfo.ProviderApiType);
            disposableDependencyObjectWrapper.Object.Initialize(externalLoginProviderInfo);
            return disposableDependencyObjectWrapper;
        }

        // Token: 0x04000008 RID: 8
        private readonly IIocResolver _iocResolver;

        // Token: 0x04000009 RID: 9
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
    }
}
