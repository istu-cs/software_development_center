using System;
using System.ComponentModel.DataAnnotations;

namespace SDC.Web.Models
{
	public class CommentModel
	{
		public long Id { get; set; }
		public DateTime Time { get; set; }
		[Required]
		[DataType(DataType.MultilineText)]
		public string Text { get; set; }
		public string AuthorId { get; set; }
		public string AuthorName { get; set; }
		public long IssueId { get; set; }
		public string IssueTitle { get; set; }
	}
}