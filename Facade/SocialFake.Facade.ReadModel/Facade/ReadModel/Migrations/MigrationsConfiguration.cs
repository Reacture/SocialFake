namespace SocialFake.Facade.ReadModel.Migrations
{
    using System.Data.Entity.Migrations;

    public sealed class MigrationsConfiguration : DbMigrationsConfiguration<SocialFakeDbContext>
    {
        public MigrationsConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Facade\ReadModel\Migrations";
        }
    }
}
