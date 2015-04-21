using System.Collections.Generic;
using Database.Entities.Enum;

namespace Database.Entities
{
	public class Issue : IdentityBase
	{
		public string Title { get; set; }
		public string Description { get; set; }

		public string AuthorId { get; set; }
		public virtual User Author { get; set; }

		public long ProjectId { get; set; }
		public virtual Project Project { get; set; }

		public long? ParentIssueId { get; set; }
		public virtual Issue ParentIssue { get; set; }

		public virtual ICollection<Comment> Comments { get; set; }
		public virtual ICollection<Issue> ChildIssues { get; set; }
		public virtual ICollection<IssueStatus> IssueStatuses { get; set; }
	}
}