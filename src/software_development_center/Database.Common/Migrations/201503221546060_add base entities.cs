namespace Database.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addbaseentities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Text = c.String(unicode: false),
                        Time = c.DateTime(nullable: false, precision: 0),
                        AuthorId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        IssueId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId)
                .ForeignKey("dbo.Issues", t => t.IssueId)
                .Index(t => t.AuthorId)
                .Index(t => t.IssueId);
            
            CreateTable(
                "dbo.Issues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Title = c.String(unicode: false),
                        Status = c.Int(nullable: false),
                        Description = c.String(unicode: false),
                        AuthorId = c.String(nullable: false, maxLength: 128, storeType: "nvarchar"),
                        PerformerId = c.String(maxLength: 128, storeType: "nvarchar"),
                        ProjectId = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthorId)
                .ForeignKey("dbo.AspNetUsers", t => t.PerformerId)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.AuthorId)
                .Index(t => t.PerformerId)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Projects",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Name = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Comments", "IssueId", "dbo.Issues");
            DropForeignKey("dbo.Comments", "AuthorId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Issues", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Issues", "PerformerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Issues", "AuthorId", "dbo.AspNetUsers");
            DropIndex("dbo.Issues", new[] { "ProjectId" });
            DropIndex("dbo.Issues", new[] { "PerformerId" });
            DropIndex("dbo.Issues", new[] { "AuthorId" });
            DropIndex("dbo.Comments", new[] { "IssueId" });
            DropIndex("dbo.Comments", new[] { "AuthorId" });
            DropTable("dbo.Projects");
            DropTable("dbo.Issues");
            DropTable("dbo.Comments");
        }
    }
}
