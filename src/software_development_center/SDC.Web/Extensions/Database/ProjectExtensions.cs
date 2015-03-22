using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions.Database
{
	public static class ProjectExtensions
	{
		public static ProjectModel ToModel(this Project project)
		{
			return new ProjectModel
			{
				Id = project.Id,
				Name = project.Name
			};
		}
	}
}