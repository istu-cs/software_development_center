using System.Linq;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions.Database
{
	public static class IssueExtensions
	{
		public static IssueViewModel ToModel(this Issue issue)
		{
			var comments = issue.Comments.ToList()
				.Select(x => x.ToModel())
				.OrderByDescending(x => x.Time)
				.ToList();

			var childIssues = issue.ChildIssues.ToList()
				.Select(x => x.ToModel())
				.ToList();

			var issueStatuses = issue.IssueStatuses
				.Select(x => new IssueStatusListItemViewModel
				{
					State = x.State,
					TeamId = x.TeamId,
					TeamName = x.Team.Name
				})
				.ToList();

			return new IssueViewModel
			{
				Id = issue.Id,
				Title = issue.Title,
				Description = issue.Description,
				AuthorId = issue.AuthorId,
				AuthorName = issue.Author.UserName,
				ProjectId = issue.ProjectId,
				ProjectName = issue.Project.Name,
				ParentIssueId = issue.ParentIssueId,
				ParentIssueTitle = issue.ParentIssueId == null ? "" : issue.ParentIssue.Title,
				Comments = comments,
				ChildIssues = childIssues,
				IssueStatuses = issueStatuses
			};
		}
	}
}