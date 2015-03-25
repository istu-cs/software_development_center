namespace Database.Common.Migrations
{
	using System;
	using System.Data.Entity.Migrations;

	public partial class EnableCascadeDelete : DbMigration
	{
		public override void Up()
		{
			// fix: sql script execution error
			//DropForeignKey("Comments", "IssueId", "Issues");
			//DropForeignKey("Issues", "ParentIssueId", "Issues");
			//DropForeignKey("Issues", "ProjectId", "Projects");
			Sql("alter table sdc_test.comments drop foreign key `FK_Comments_Issues_IssueId`");
			Sql("alter table sdc_test.issues drop foreign key `FK_Issues_Issues_ParentIssueId`");
			Sql("alter table sdc_test.issues drop foreign key `FK_Issues_Projects_ProjectId`");

			AddForeignKey("Comments", "IssueId", "Issues", "Id", cascadeDelete: true);
			AddForeignKey("Issues", "ParentIssueId", "Issues", "Id", cascadeDelete: true);
			AddForeignKey("Issues", "ProjectId", "Projects", "Id", cascadeDelete: true);
		}

		public override void Down()
		{
			// fix: sql script execution error
			//DropForeignKey("Issues", "ProjectId", "Projects");
			//DropForeignKey("Issues", "ParentIssueId", "Issues");
			//DropForeignKey("Comments", "IssueId", "Issues");
			Sql("alter table sdc_test.comments drop foreign key `FK_Comments_Issues_IssueId`");
			Sql("alter table sdc_test.issues drop foreign key `FK_Issues_Issues_ParentIssueId`");
			Sql("alter table sdc_test.issues drop foreign key `FK_Issues_Projects_ProjectId`");

			AddForeignKey("Issues", "ProjectId", "Projects", "Id");
			AddForeignKey("Issues", "ParentIssueId", "Issues", "Id");
			AddForeignKey("Comments", "IssueId", "Issues", "Id");
		}
	}
}
