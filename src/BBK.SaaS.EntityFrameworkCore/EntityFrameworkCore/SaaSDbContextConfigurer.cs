using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace BBK.SaaS.EntityFrameworkCore
{
    public static class SaaSDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<SaaSDbContext> builder, string connectionString)
        {
            //builder.UseSqlServer(connectionString);
            builder.UseNpgsql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<SaaSDbContext> builder, DbConnection connection)
        {
            //builder.UseSqlServer(connection);
            builder.UseNpgsql(connection);
        }
    }
}