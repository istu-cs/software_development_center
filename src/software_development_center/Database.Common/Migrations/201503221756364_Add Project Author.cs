namespace Database.Common.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectAuthor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "AuthorId", c => c.String(nullable: false, maxLength: 128, storeType: "nvarchar"));
            CreateIndex("dbo.Projects", "AuthorId");
            AddForeignKey("dbo.Projects", "AuthorId", "dbo.AspNetUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Projects", "AuthorId", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "AuthorId" });
            DropColumn("dbo.Projects", "AuthorId");
        }
    }
}
