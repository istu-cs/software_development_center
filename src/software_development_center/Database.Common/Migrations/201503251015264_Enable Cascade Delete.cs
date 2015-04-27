namespace Database.Common.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class EnableCascadeDelete : DbMigration
	{
		public override void Up()
		{
			DropForeignKey("Comments", "IssueId", "Issues");
			DropForeignKey("Issues", "ParentIssueId", "Issues");
			DropForeignKey("Issues", "ProjectId", "Projects");

			AddForeignKey("Comments", "IssueId", "Issues", "Id", cascadeDelete: true);
			AddForeignKey("Issues", "ParentIssueId", "Issues", "Id", cascadeDelete: true);
			AddForeignKey("Issues", "ProjectId", "Projects", "Id", cascadeDelete: true);
		}

		public override void Down()
		{
			DropForeignKey("Issues", "ProjectId", "Projects");
			DropForeignKey("Issues", "ParentIssueId", "Issues");
			DropForeignKey("Comments", "IssueId", "Issues");

			AddForeignKey("Issues", "ProjectId", "Projects", "Id");
			AddForeignKey("Issues", "ParentIssueId", "Issues", "Id");
			AddForeignKey("Comments", "IssueId", "Issues", "Id");
		}
	}
}
