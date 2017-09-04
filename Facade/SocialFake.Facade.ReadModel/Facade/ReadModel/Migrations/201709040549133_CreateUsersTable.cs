namespace SocialFake.Facade.ReadModel.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateUsersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SequenceId = c.Long(nullable: false, identity: true),
                        Username = c.String(maxLength: 100),
                        DisplayNamesJson = c.String(),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.SequenceId, clustered: true)
                .Index(t => t.Username);
        }

        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.Users", new[] { "SequenceId" });
            DropTable("dbo.Users");
        }
    }
}
