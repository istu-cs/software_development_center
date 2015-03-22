using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions
{
	public static class CommentModelExtensions
	{
		public static Comment ToDbModel(this CommentModel model)
		{
			return new Comment
			{
				Id = model.Id,
				Text = model.Text,
				Time = model.Time,
				AuthorId = model.AuthorId,
				IssueId = model.IssueId
			};
		}
	}
}