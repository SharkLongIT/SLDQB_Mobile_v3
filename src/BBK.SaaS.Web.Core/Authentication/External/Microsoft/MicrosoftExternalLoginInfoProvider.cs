using System.Collections.Generic;

namespace BBK.SaaS.Web.Authentication.External.Microsoft;

public class MicrosoftExternalLoginInfoProvider : IExternalLoginInfoProvider
{
    public string Name { get; } = "Microsoft";


    protected string ConsumerKey { get; set; }

    protected string ConsumerSecret { get; set; }

    protected Dictionary<string, string> AdditionalParameters { get; set; }

    protected ExternalLoginProviderInfo ExternalLoginProviderInfo { get; set; }

    public MicrosoftExternalLoginInfoProvider(string consumerKey, string consumerSecret, Dictionary<string, string> additionalParameters = null)
    {
        ConsumerKey = consumerKey;
        ConsumerSecret = consumerSecret;
        AdditionalParameters = additionalParameters;
        CreateExternalLoginInfo();
    }

    private void CreateExternalLoginInfo()
    {
        ExternalLoginProviderInfo = new ExternalLoginProviderInfo("Microsoft", ConsumerKey, ConsumerSecret, typeof(MicrosoftAuthProviderApi), AdditionalParameters);
    }

    public virtual ExternalLoginProviderInfo GetExternalLoginInfo()
    {
        return ExternalLoginProviderInfo;
    }
}
