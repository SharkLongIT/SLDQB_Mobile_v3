using System.Collections.Generic;

namespace BBK.SaaS.Web.Authentication.External.WsFederation;

public class WsFederationExternalLoginInfoProvider : IExternalLoginInfoProvider
{
    public string Name { get; } = "WsFederation";


    protected string ClientId { get; set; }

    protected string Tenant { get; set; }

    protected string MetaDataAddress { get; set; }

    protected string Authority { get; set; }

    protected List<JsonClaimMap> JsonClaimMaps { get; set; }

    protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }

    public WsFederationExternalLoginInfoProvider(string clientId, string tenant, string metaDataAddress, string authority, List<JsonClaimMap> jsonClaimMaps = null)
    {
        ClientId = clientId;
        Tenant = tenant;
        MetaDataAddress = metaDataAddress;
        Authority = authority;
        JsonClaimMaps = jsonClaimMaps;
        CreateExternalLoginProviderInfo();
    }

    private void CreateExternalLoginProviderInfo()
    {
        ExternalLoginProviderInfo = new ExternalLoginProviderInfo("WsFederation", ClientId, "", typeof(WsFederationAuthProviderApi), new Dictionary<string, string>
        {
            { "Tenant", Tenant },
            { "MetaDataAddress", MetaDataAddress },
            { "Authority", Authority }
        }, JsonClaimMaps);
    }

    public virtual ExternalLoginProviderInfo GetExternalLoginInfo()
    {
        return ExternalLoginProviderInfo;
    }
}
