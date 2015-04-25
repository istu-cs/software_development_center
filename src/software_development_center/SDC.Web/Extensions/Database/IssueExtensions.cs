using System.Linq;
using System.Net.Http;
using System.Web;
using Database.Entities;
using Database.Entities.Enum;
using SDC.Web.Models;
using SDC.Web.Types;

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

			var user = (BasePrincipal) HttpContext.Current.User;
			var currentTeamIssueState = IssueState.Unoccupied;
			if (user.Identity.IsAuthenticated)
			{
				var currentTeamId = user.GetCurrentTeamId();
				var status = issueStatuses.SingleOrDefault(x => x.TeamId == currentTeamId);
				currentTeamIssueState = status == null ? IssueState.Unoccupied : status.State;
			}

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
				IssueStatuses = issueStatuses,
				CurrentTeamIssueState = currentTeamIssueState
			};
		}
	}
}