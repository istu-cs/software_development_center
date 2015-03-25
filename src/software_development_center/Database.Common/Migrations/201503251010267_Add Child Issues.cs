namespace Database.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChildIssues : DbMigration
    {
        public override void Up()
        {
            AddColumn("Issues", "ParentIssueId", c => c.Long());
            CreateIndex("Issues", "ParentIssueId");
            AddForeignKey("Issues", "ParentIssueId", "Issues", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("Issues", "ParentIssueId", "Issues");
            DropIndex("Issues", new[] { "ParentIssueId" });
            DropColumn("Issues", "ParentIssueId");
        }
    }
}
