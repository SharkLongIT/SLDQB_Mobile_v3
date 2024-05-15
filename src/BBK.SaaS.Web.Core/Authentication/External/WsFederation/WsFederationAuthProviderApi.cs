using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.WsFederation;
using Microsoft.IdentityModel.Tokens;

namespace BBK.SaaS.Web.Authentication.External.WsFederation
{
    public class WsFederationAuthProviderApi : ExternalAuthProviderApiBase
    {
        public const string Name = "WsFederation";

        public override async Task<ExternalAuthUserInfo> GetUserInfo(string token)
        {
            string issuer = base.ProviderInfo.AdditionalParams["Authority"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ApplicationException("Authentication:WsFederation:Issuer configuration is required.");
            }

            string metadata = base.ProviderInfo.AdditionalParams["MetaDataAddress"];
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ApplicationException("Authentication:WsFederation:MetaDataAddress configuration is required.");
            }

            ConfigurationManager<WsFederationConfiguration> configurationManager = new ConfigurationManager<WsFederationConfiguration>(metadata, new WsFederationConfigurationRetriever(), new HttpDocumentRetriever());
            JwtSecurityToken validatedToken = await ValidateToken(token, issuer, configurationManager);
            string fullName = validatedToken.Claims.First((Claim c) => c.Type == "name").Value;
            string email = validatedToken.Claims.First((Claim c) => c.Type == "email").Value;
            string[] fullNameParts = fullName.Split(' ');
            return new ExternalAuthUserInfo
            {
                Provider = "WsFederation",
                ProviderKey = validatedToken.Subject,
                Name = fullNameParts[0],
                Surname = ((fullNameParts.Length > 1) ? fullNameParts[1] : fullNameParts[0]),
                EmailAddress = email,
                Claims = validatedToken.Claims.Select((Claim c) => new ClaimKeyValue(c.Type, c.Value)).ToList()
            };
        }

        private async Task<JwtSecurityToken> ValidateToken(string token, string issuer, IConfigurationManager<WsFederationConfiguration> configurationManager, CancellationToken ct = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException("token");
            }

            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException("issuer");
            }

            ICollection<SecurityKey> signingKeys = (await configurationManager.GetConfigurationAsync(ct)).SigningKeys;
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = signingKeys,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5.0),
                ValidateAudience = false
            };
            SecurityToken rawValidatedToken;
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(token, validationParameters, out rawValidatedToken);
            principal.AddMappedClaims(base.ProviderInfo.ClaimMappings);
            if (base.ProviderInfo.ClientId != principal.Claims.First((Claim c) => c.Type == "aud").Value)
            {
                throw new ApplicationException("ClientId couldn't verified.");
            }

            return (JwtSecurityToken)rawValidatedToken;
        }
    }
}
