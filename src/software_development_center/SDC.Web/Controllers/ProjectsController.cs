using System.Linq;
using System.Net;
using System.Web.Mvc;
using Database.Common;
using Microsoft.AspNet.Identity;
using SDC.Web.Extensions;
using SDC.Web.Extensions.Database;
using SDC.Web.Models;

namespace SDC.Web.Controllers
{
	public class ProjectsController : BaseController
	{
		public ActionResult Index()
		{
			var projects = db.Projects.ToList().Select(x => x.ToModel());
			return View(projects.ToList());
		}
		
		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var project = db.Projects.Find(id);
			if (project == null)
			{
				return HttpNotFound();
			}
			return View(project.ToModel());
		}

		[Authorize]
		public ActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize]
		public ActionResult Create(ProjectModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var project = model.ToDbModel();
			project.AuthorId = User.Identity.GetUserId();
			db.Projects.Add(project);
			db.SaveChanges();
			return RedirectToAction("Details", new {id = project.Id});
		}

		[Authorize]
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var project = db.Projects.Find(id);
			if (project == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (project.AuthorId != currentUserId)
			{
				return RedirectToAction("Index");
			}

			return View(project.ToModel());
		}

		[HttpPost]
		[Authorize]
		public ActionResult Edit(ProjectModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var project = db.Projects.Find(model.Id);
			var currentUserId = User.Identity.GetUserId();
			if (project.AuthorId != currentUserId)
			{
				return RedirectToAction("Index");
			}
			db.Entry(project).CurrentValues.SetValues(model.ToDbModel());
			db.SaveChanges();
			return RedirectToAction("Details", new { id = project.Id });
		}

		[Authorize]
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var project = db.Projects.Find(id);
			if (project == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (project.AuthorId != currentUserId)
			{
				return RedirectToAction("Index");
			}

			return View(project.ToModel());
		}

		[HttpPost, ActionName("Delete")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			var project = db.Projects.Find(id);
			var currentUserId = User.Identity.GetUserId();
			if (project.AuthorId == currentUserId)
			{
				//db.Comments.RemoveRange(project.Issues.SelectMany(x => x.Comments));
				//db.Issues.RemoveRange(project.Issues);
				db.Projects.Remove(project);
				db.SaveChanges();
			}

			return RedirectToAction("Index");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}