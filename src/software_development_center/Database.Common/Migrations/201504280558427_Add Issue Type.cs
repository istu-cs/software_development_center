namespace Database.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIssueType : DbMigration
    {
        public override void Up()
        {
            AddColumn("Issues", "Type", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("Issues", "Type");
        }
    }
}
