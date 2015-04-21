using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Database.Common;
using Microsoft.AspNet.Identity;

namespace SDC.Web.Controllers
{
	public class UsersController : Controller
	{
		private SdcDbContext db = SdcDbContext.Create();

		// GET: Users
		public ActionResult Index(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return HttpNotFound();
			}

			if (id == User.Identity.GetUserId())
			{
				return RedirectToAction("Index", "Manage");
			}

			var user = db.Users.Find(id);
			if (user == null)
			{
				return HttpNotFound();
			}

			throw new NotImplementedException();

			return View();
		}
	}
}