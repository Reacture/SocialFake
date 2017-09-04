namespace SocialFake.Facade.ReadModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBioColumnToUsersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Bio", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Bio");
        }
    }
}
