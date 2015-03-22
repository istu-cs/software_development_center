using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Database.Entities.Enum;

namespace Database.Entities
{
	public class Issue : IdentityBase
	{
		public string Title { get; set; }
		public IssueStatus Status { get; set; }
		public string Description { get; set; }

		public string AuthorId { get; set; }
		public virtual ApplicationUser Author { get; set; }

		public string PerformerId { get; set; }
		public virtual ApplicationUser Performer { get; set; }

		public long ProjectId { get; set; }
		public virtual Project Project { get; set; }

		public virtual ICollection<Comment> Comments { get; set; }
	}
}