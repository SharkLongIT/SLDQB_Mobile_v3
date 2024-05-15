using BBK.SaaS.Configuration;
using BBK.SaaS.Web.Authentication.External;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BBK.SaaS.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    public class TwitterController : SaaSControllerBase
    {
        private readonly ExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IConfigurationRoot _appConfiguration;
        
        public TwitterController(
            ExternalAuthConfiguration externalAuthConfiguration, 
            IAppConfigurationAccessor appConfigurationAccessor)
        {
            _externalAuthConfiguration = externalAuthConfiguration;
            _appConfiguration = appConfigurationAccessor.Configuration;
        }

        //[HttpPost]
        //public async Task<TwitterGetRequestTokenResponse> GetRequestToken()
        //{
        //    var loginInfoProvider = _externalAuthConfiguration.ExternalLoginInfoProviders.FirstOrDefault(
        //        e => e.Name == TwitterAuthProviderApi.Name
        //    );

        //    if (loginInfoProvider == null)
        //    {
        //        throw new UserFriendlyException("Twitter login configuration is missing !");
        //    }

        //    var loginInfo = loginInfoProvider.GetExternalLoginInfo();
        //    var callbackUrl = _appConfiguration["App:ClientRootAddress"].EnsureEndsWith('/') + "account/login";
            
        //    var twitter = new TwitterAuthProviderApi();
        //    return await twitter.GetRequestToken(
        //        loginInfo.ClientId,
        //        loginInfo.ClientSecret,
        //        callbackUrl);
        //}

        //[HttpPost]
        //public async Task<TwitterGetAccessTokenResponse> GetAccessToken(string token, string verifier)
        //{
        //    var twitter = new TwitterAuthProviderApi();
        //    return await twitter.GetAccessToken(token, verifier);
        //}
    }
}
