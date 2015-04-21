using System;

namespace Database.Entities
{
	public class Comment : IdentityBase
	{
		public string Text { get; set; }
		public DateTime Time { get; set; }

		public string AuthorId { get; set; }
		public virtual User Author { get; set; }

		public long IssueId { get; set; }
		public virtual Issue Issue { get; set; }
	}
}
