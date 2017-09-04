namespace SocialFake.Facade.ReadModel.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class CreateCorrelationsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Correlations",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        SequenceId = c.Long(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.Id, clustered: false)
                .Index(t => t.SequenceId, clustered: true);
        }

        public override void Down()
        {
            DropIndex("dbo.Correlations", new[] { "SequenceId" });
            DropTable("dbo.Correlations");
        }
    }
}
