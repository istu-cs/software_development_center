using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SDC.Web.Models
{
	public class IndexTeamViewModel
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public IList<UserListItemViewModel> Performers;
		public IList<IssueListItemViewModel> Issues;
	}

	public class IssueListItemViewModel
	{
		public long Id { get; set; }
		public string Title { get; set; }
	}

	public class TeamListItemViewModel
	{
		public long Id { get; set; }
		public string Name { get; set; }
	}

	public class CreateTeamViewModel
	{
		[Required]
		public string Name { get; set; }
	}

	public class EditTeamViewModel
	{
		public long Id { get; set; }
		[Required]
		public string Name { get; set; }

		public IList<UserListItemViewModel> Performers { get; set; }
	}

	public class AddPerformerTeamViewModel
	{
		public long TeamId { get; set; }
		[Required]
		public string PerformerId { get; set; }
	}

	public class DeletePerformerTeamViewModel
	{
		public long TeamId { get; set; }
		[Required]
		public string PerformerId { get; set; }
	}

	public class ChangeCurrentTeamViewModel
	{
		public long TeamId { get; set; }
		public IEnumerable<SelectListItem> Teams { get; set; }
	}
}