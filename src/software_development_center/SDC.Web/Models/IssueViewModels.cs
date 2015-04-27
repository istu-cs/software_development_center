using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Database.Entities;
using Database.Entities.Enum;

namespace SDC.Web.Models
{
	public class IndexIssueViewModel
	{
		public long ProjectId { get; set; }
		public IEnumerable<IssueListItemViewModel> Issues { get; set; }
	}

	public class DetailsIssueViewModel
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public IssueStatus Status { get; set; }
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public string AuthorId { get; set; }
		public string AuthorName { get; set; }
		public long ProjectId { get; set; }
		public string ProjectName { get; set; }
		public long? ParentIssueId { get; set; }
		public string ParentIssueTitle { get; set; }

		public bool CurrentUserCanEdit { get; set; }
		public bool CurrentTeamCanAssign { get; set; }
		public bool CurrentTeamCanUnassign { get; set; }
		public bool CurrentTeamCanStartProgress { get; set; }
		public bool CurrentTeamCanSendToReview { get; set; }
		public TeamProgressStatus CurrentTeamProgressStatus { get; set; }

		public IEnumerable<IssueListItemViewModel> ChildIssues { get; set; }
		public IEnumerable<TeamProgressListItemViewModel> TeamsProgress { get; set; }
	}

	public class CreateIssueViewModel
	{
		[Required]
		public string Title { get; set; }
		[Required]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public long ProjectId { get; set; }
		public long? ParentIssueId { get; set; }
	}

	public class EditIssueViewModel
	{
		public long Id { get; set; }
		[Required]
		public string Title { get; set; }
		[Required]
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public long ProjectId { get; set; }
	}

	public class DeleteIssueViewModel
	{
		public long Id { get; set; }
		public string Title { get; set; }
		[DataType(DataType.MultilineText)]
		public string Description { get; set; }
		public string AuthorName { get; set; }
		public string ProjectName { get; set; }
	}

	public class IssueListItemViewModel
	{
		public long Id { get; set; }
		public string Title { get; set; }
	}

	public class TeamProgressListItemViewModel
	{
		public TeamProgressStatus Status { get; set; }
		public long TeamId { get; set; }
		public string TeamName { get; set; }
	}
}