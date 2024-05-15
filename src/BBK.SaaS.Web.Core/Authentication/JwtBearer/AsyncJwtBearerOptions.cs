using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BBK.SaaS.Web.Authentication.JwtBearer
{
    public class AsyncJwtBearerOptions : JwtBearerOptions
    {
        public readonly List<IAsyncSecurityTokenValidator> AsyncSecurityTokenValidators;
        
        private readonly SaaSAsyncJwtSecurityTokenHandler _defaultAsyncHandler = new SaaSAsyncJwtSecurityTokenHandler();

        public AsyncJwtBearerOptions()
        {
            AsyncSecurityTokenValidators = new List<IAsyncSecurityTokenValidator>() {_defaultAsyncHandler};
        }
    }

}
