using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions
{
	public static class IssueModelExtensions
	{
		public static Issue ToDbModel(this IssueModel model)
		{
			return new Issue
			{
				Id = model.Id,
				Title = model.Title,
				Status = model.Status,
				Description = model.Description,
				ProjectId = model.ProjectId,
				AuthorId = model.AuthorId,
				PerformerId = model.PerformerId,
				ParentIssueId = model.ParentIssueId
			};
		}
	}
}