namespace Database.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeams : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Issues", "PerformerId", "AspNetUsers");
            DropForeignKey("Issues", "ParentIssueId", "Issues");
            DropForeignKey("Issues", "ProjectId", "Projects");
            DropIndex("Issues", new[] { "PerformerId" });
            CreateTable(
                "IssueStatus",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        TeamId = c.Long(nullable: false),
                        IssueId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .ForeignKey("Issues", t => t.IssueId)
                .ForeignKey("Teams", t => t.TeamId)
                .Index(t => new { t.TeamId, t.IssueId }, unique: true, name: "IX_TeamAndIssue");
            
            CreateTable(
                "Teams",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(maxLength: 255, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)                
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "TeamUsers",
                c => new
                    {
                        Team_Id = c.Long(nullable: false),
                        User_Id = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => new { t.Team_Id, t.User_Id })                
                .ForeignKey("Teams", t => t.Team_Id, cascadeDelete: true)
                .ForeignKey("AspNetUsers", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Team_Id)
                .Index(t => t.User_Id);
            
            AddForeignKey("Issues", "ParentIssueId", "Issues", "Id");
            AddForeignKey("Issues", "ProjectId", "Projects", "Id");
            DropColumn("Issues", "Status");
            DropColumn("Issues", "PerformerId");
        }
        
        public override void Down()
        {
            AddColumn("Issues", "PerformerId", c => c.String(maxLength: 128, storeType: "nvarchar"));
            AddColumn("Issues", "Status", c => c.Int(nullable: false));
            DropForeignKey("Issues", "ProjectId", "Projects");
            DropForeignKey("Issues", "ParentIssueId", "Issues");
            DropForeignKey("IssueStatus", "TeamId", "Teams");
            DropForeignKey("TeamUsers", "User_Id", "AspNetUsers");
            DropForeignKey("TeamUsers", "Team_Id", "Teams");
            DropForeignKey("IssueStatus", "IssueId", "Issues");
            DropIndex("TeamUsers", new[] { "User_Id" });
            DropIndex("TeamUsers", new[] { "Team_Id" });
            DropIndex("Teams", new[] { "Name" });
            DropIndex("IssueStatus", "IX_TeamAndIssue");
            DropTable("TeamUsers");
            DropTable("Teams");
            DropTable("IssueStatus");
            CreateIndex("Issues", "PerformerId");
            AddForeignKey("Issues", "ProjectId", "Projects", "Id", cascadeDelete: true);
            AddForeignKey("Issues", "ParentIssueId", "Issues", "Id", cascadeDelete: true);
            AddForeignKey("Issues", "PerformerId", "AspNetUsers", "Id");
        }
    }
}
