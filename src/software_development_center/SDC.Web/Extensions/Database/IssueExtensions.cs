using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using Database.Entities.Enum;
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

			return new IssueModel
			{
				Id = issue.Id,
				Title = issue.Title,
				Status = issue.Status,
				Description = issue.Description,
				AuthorId = issue.AuthorId,
				AuthorName = issue.Author.UserName,
				PerformerId = issue.PerformerId,
				PerformerName = issue.PerformerId == null ? null : issue.Performer.UserName,
				ProjectId = issue.ProjectId,
				ProjectName = issue.Project.Name,
				Comments = comments
			};
		}
	}
}