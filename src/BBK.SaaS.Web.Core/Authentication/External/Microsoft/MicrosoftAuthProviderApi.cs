using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Newtonsoft.Json.Linq;

namespace BBK.SaaS.Web.Authentication.External.Microsoft
{
    public class MicrosoftAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "Microsoft";

        public override async Task<ExternalAuthUserInfo> GetUserInfo(string accessCode)
        {
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Microsoft ASP.NET Core OAuth middleware");
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
            client.Timeout = TimeSpan.FromSeconds(30.0);
            client.MaxResponseContentBufferSize = 10485760L;
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, MicrosoftAccountDefaults.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessCode);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            JObject payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            ExternalAuthUserInfo result = new ExternalAuthUserInfo
            {
                Name = MicrosoftAccountHelper.GetGivenName(payload),
                EmailAddress = MicrosoftAccountHelper.GetEmail(payload),
                Surname = MicrosoftAccountHelper.GetSurname(payload),
                Provider = "Microsoft",
                ProviderKey = MicrosoftAccountHelper.GetId(payload)
            };
            FillClaimsFromJObject(result, payload);
            return result;
        }
    }
}
