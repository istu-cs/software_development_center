using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDC.Web.Models
{
	public class ProjectModel
	{
		public long Id { get; set; }
		[Required]
		public string Name { get; set; }
	}
}