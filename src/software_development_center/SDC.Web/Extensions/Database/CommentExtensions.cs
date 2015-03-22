using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions.Database
{
	public static class CommentExtensions
	{
		public static CommentModel ToModel(this Comment comment)
		{
			return new CommentModel
			{
				Id = comment.Id,
				Time = comment.Time,
				Text = comment.Text,
				AuthorId = comment.AuthorId,
				AuthorName = comment.Author.UserName,
				IssueId = comment.IssueId,
				IssueTitle = comment.Issue.Title
			};
		}
	}
}