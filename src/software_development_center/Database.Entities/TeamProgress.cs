using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Entities.Enum;

namespace Database.Entities
{
	public class TeamProgress : IdentityBase
	{
		public TeamProgressStatus Status { get; set; }

		[Index("IX_TeamAndIssue", 1, IsUnique = true)]
		public long TeamId { get; set; }
		public virtual Team Team { get; set; }

		[Index("IX_TeamAndIssue", 2, IsUnique = true)]
		public long IssueId { get; set; }
		public virtual Issue Issue { get; set; }
	}
}
