namespace Database.Common.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class AddIssueStatus : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("IssueStatus", "IssueId", "Issues");
			DropForeignKey("IssueStatus", "TeamId", "Teams");
			RenameTable(name: "IssueStatus", newName: "TeamProgresses");
			AddColumn("Issues", "Status", c => c.Int(nullable: false));
			AddColumn("TeamProgresses", "Status", c => c.Int(nullable: false));
			AddForeignKey("TeamProgresses", "IssueId", "Issues", "Id", cascadeDelete: true);
			AddForeignKey("TeamProgresses", "TeamId", "Teams", "Id");
			DropColumn("TeamProgresses", "State");
		}

		public override void Down()
		{
			AddColumn("TeamProgresses", "State", c => c.Int(nullable: false));
			DropForeignKey("TeamProgresses", "TeamId", "Teams");
			DropForeignKey("TeamProgresses", "IssueId", "Issues");
			DropColumn("TeamProgresses", "Status");
			DropColumn("Issues", "Status");
			RenameTable(name: "TeamProgresses", newName: "IssueStatus");
			AddForeignKey("IssueStatus", "TeamId", "Teams", "Id");
			AddForeignKey("IssueStatus", "IssueId", "Issues", "Id");
		}
	}
}