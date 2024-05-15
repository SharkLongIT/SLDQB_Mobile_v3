using System;
using Newtonsoft.Json.Linq;

namespace BBK.SaaS.Web.Authentication.External.Microsoft
{
    public static class MicrosoftAccountHelper
    {
        public static string GetId(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.Value<string>("id");
        }

        public static string GetDisplayName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.Value<string>("displayName");
        }

        public static string GetGivenName(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.Value<string>("givenName");
        }

        public static string GetSurname(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.ContainsKey("surname") ? user.Value<string>("surname") : string.Empty;
        }

        public static string GetEmail(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return user.Value<string>("mail") ?? user.Value<string>("userPrincipalName");
        }
    }
}
