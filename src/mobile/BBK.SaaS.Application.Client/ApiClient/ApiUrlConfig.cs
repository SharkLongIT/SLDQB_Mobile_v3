using System;
using Abp.Extensions;

namespace BBK.SaaS.ApiClient
{
    public static class ApiUrlConfig
    {

     //   private const string DefaultHostUrl = "https://bpm.bbksolution.com/"; //TODO: Replace with PROD WebAPI URL.
        private const string DefaultHostUrl = "http://192.168.0.248:32080/"; //TODO: Replace with PROD WebAPI URL.
       // private const string DefaultHostUrl = "https://test.bbksolution.com/"; //TODO: Replace with PROD WebAPI URL.
       //private const string DefaultHostUrl = "http://192.168.0.248:31080/"; //TODO: Replace with PROD WebAPI URL.
      // private const string DefaultHostUrl = "https://localhost:44303/"; //TODO: Replace with PROD WebAPI URL.

        public static string BaseUrl { get; private set; }

        static ApiUrlConfig()   
        {
            ResetBaseUrl();
        }

        public static void ChangeBaseUrl(string baseUrl)
        {
            BaseUrl = ReplaceLocalhost(NormalizeUrl(baseUrl));
        }

        public static void ResetBaseUrl()
        {
            BaseUrl = ReplaceLocalhost(DefaultHostUrl);
        }

        public static bool IsLocal => DefaultHostUrl.Contains("localhost") || DefaultHostUrl.Contains("127.0.0.1");

        private static string NormalizeUrl(string baseUrl)
        {
            if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != "http" && uriResult.Scheme != "https"))
            {
                throw new ArgumentException("Unexpected base URL: " + baseUrl);
            }
            return uriResult.ToString().EnsureEndsWith('/');
        }

        private static string ReplaceLocalhost(string url)
        {
            return url.Replace("localhost", DebugServerIpAddresses.Current);
        }
    }
}