using System.Data.Entity;

namespace SocialFake.Facade.ReadModel
{
    public class SocialFakeDbContext : DbContext
    {
        public SocialFakeDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public SocialFakeDbContext()
            : this(nameof(SocialFakeDbContext))
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Correlation> Correlations { get; set; }
    }
}
