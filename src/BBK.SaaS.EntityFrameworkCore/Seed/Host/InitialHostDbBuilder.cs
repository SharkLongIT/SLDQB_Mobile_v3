using BBK.SaaS.EntityFrameworkCore;

namespace BBK.SaaS.Migrations.Seed.Host
{
    public class InitialHostDbBuilder
    {
        private readonly SaaSDbContext _context;

        public InitialHostDbBuilder(SaaSDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultEditionCreator(_context).Create();
            new DefaultLanguagesCreator(_context).Create();
            new HostRoleAndUserCreator(_context).Create();
            new DefaultSettingsCreator(_context).Create();

            _context.SaveChanges();
        }
    }
}
