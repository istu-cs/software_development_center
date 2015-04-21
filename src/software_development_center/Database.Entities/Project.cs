using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
	public class Project : IdentityBase
	{
		public string Name { get; set; }

		public string AuthorId { get; set; }
		public virtual User Author { get; set; }

		public virtual ICollection<Issue> Issues { get; set; }
	}
}