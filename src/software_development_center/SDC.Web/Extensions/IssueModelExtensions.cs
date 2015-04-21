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
		public static Issue ToDbModel(this IssueViewModel model)
		{
			return new Issue
			{
				Id = model.Id,
				Title = model.Title,
				Description = model.Description,
				ProjectId = model.ProjectId,
				AuthorId = model.AuthorId,
				ParentIssueId = model.ParentIssueId
			};
		}
	}
}