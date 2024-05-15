using System.Linq;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using BBK.SaaS.Editions;
using BBK.SaaS.EntityFrameworkCore;

namespace BBK.SaaS.Migrations.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly SaaSDbContext _context;

        public DefaultTenantBuilder(SaaSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }
        private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.IgnoreQueryFilters().FirstOrDefault(t => t.TenancyName == AbpTenantBase.DefaultTenantName);
            if (defaultTenant == null)
            {
                defaultTenant = new MultiTenancy.Tenant(AbpTenantBase.DefaultTenantName, AbpTenantBase.DefaultTenantName);

                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    defaultTenant.EditionId = defaultEdition.Id;
                }

                _context.Tenants.Add(defaultTenant);
                _context.SaveChanges();
            }
        }
    }
}
