using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Database.Entities;
using Database.Entities.Enum;

namespace SDC.Web.Models
{
	public class IssueViewModel
	{
		public long Id { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public string AuthorId { get; set; }
		public string AuthorName { get; set; }
		public string PerformerId { get; set; }
		public string PerformerName { get; set; }
		[Required]
		public long ProjectId { get; set; }
		public string ProjectName { get; set; }
		public long? ParentIssueId { get; set; }
		public string ParentIssueTitle { get; set; }

		public IList<CommentModel> Comments { get; set; }
		public IList<IssueViewModel> ChildIssues { get; set; }
		public IList<IssueStatusListItemViewModel> IssueStatuses { get; set; }
	}

	public class IssueStatusListItemViewModel
	{
		public IssueState State { get; set; }
		public long TeamId { get; set; }
		public string TeamName { get; set; }
	}
}