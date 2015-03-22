using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Entities;
using SDC.Web.Models;

namespace SDC.Web.Extensions
{
	public static class ProjectModelExtensions
	{
		public static Project ToDbModel(this ProjectModel model)
		{
			return new Project
			{
				Id = model.Id,
				Name = model.Name,
				AuthorId = model.AuthorId
			};
		}
	}
}