using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Entities.Enum;

namespace Database.Entities
{
	public class Team : IdentityBase
	{
		[MaxLength(255)]
		[Index(IsUnique = true)]
		public string Name { get; set; }
		public TeamType Type { get; set; }

		public virtual ICollection<User> Performers { get; set; }
		public virtual ICollection<IssueStatus> IssueStatuses { get; set; }
	}
}
