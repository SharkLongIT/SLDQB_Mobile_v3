using Microsoft.Extensions.DependencyInjection;
using BBK.SaaS.HealthChecks;

namespace BBK.SaaS.Web.HealthCheck
{
    public static class AbpZeroHealthCheck
    {
        public static IHealthChecksBuilder AddAbpZeroHealthCheck(this IServiceCollection services)
        {
            var builder = services.AddHealthChecks();
            //builder.AddCheck<SaaSDbContextHealthCheck>("Database Connection");
            //builder.AddCheck<SaaSDbContextUsersHealthCheck>("Database Connection with user check");
            builder.AddCheck<CacheHealthCheck>("Cache");

            // add your custom health checks here
            // builder.AddCheck<MyCustomHealthCheck>("my health check");

            return builder;
        }
    }
}
