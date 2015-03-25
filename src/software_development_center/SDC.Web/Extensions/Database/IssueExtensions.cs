using System.Linq;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions.Database
{
	public static class IssueExtensions
	{
		public static IssueModel ToModel(this Issue issue)
		{
			var comments = issue.Comments.ToList()
				.Select(x => x.ToModel())
				.OrderByDescending(x => x.Time)
				.ToList();

			var childIssues = issue.ChildIssues.ToList()
				.Select(x => x.ToModel())
				.ToList();

			return new IssueModel
			{
				Id = issue.Id,
				Title = issue.Title,
				Status = issue.Status,
				Description = issue.Description,
				AuthorId = issue.AuthorId,
				AuthorName = issue.Author.UserName,
				PerformerId = issue.PerformerId,
				PerformerName = issue.PerformerId == null ? "" : issue.Performer.UserName,
				ProjectId = issue.ProjectId,
				ProjectName = issue.Project.Name,
				ParentIssueId = issue.ParentIssueId,
				ParentIssueTitle = issue.ParentIssueId == null ? "" : issue.ParentIssue.Title,
				Comments = comments,
				ChildIssues = childIssues
			};
		}
	}
}