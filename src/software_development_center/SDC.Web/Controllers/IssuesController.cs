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
	public class IssuesController : Controller
	{
		private SdcDbContext db = SdcDbContext.Create();
		
		public ActionResult Index(long? projectId)
		{
			if (projectId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			if (!db.Projects.Any(x => x.Id == projectId))
			{
				return HttpNotFound();
			}

			ViewBag.ProjectId = projectId;
			var issues = db.Issues.Where(x => x.ProjectId == projectId).ToList()
				.Select(x => x.ToModel())
				.ToList();

			return View(issues);
		}

		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return HttpNotFound();
			}
			return View(issue.ToModel());
		}

		[Authorize]
		public ActionResult Create(long? projectId)
		{
			if (projectId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			if (!db.Projects.Any(x => x.Id == projectId))
			{
				return HttpNotFound();
			}

			var model = new IssueModel
			{
				ProjectId = projectId.Value
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IssueModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var issue = model.ToDbModel();
			issue.AuthorId = User.Identity.GetUserId();

			db.Issues.Add(issue);
			db.SaveChanges();
			return RedirectToAction("Details", new {id = issue.Id});
		}

		[Authorize]
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new {projectId = issue.ProjectId});
			}

			return View(issue.ToModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(IssueModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var issue = db.Issues.Find(model.Id);
			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new { projectId = issue.ProjectId });
			}
			db.Entry(issue).CurrentValues.SetValues(model.ToDbModel());
			db.SaveChanges();
			return RedirectToAction("Details", new {id = issue.Id});
		}

		[Authorize]
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new { projectId = issue.ProjectId });
			}

			return View(issue.ToModel());
		}

		[HttpPost, ActionName("Delete")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			var issue = db.Issues.Find(id);
			var projectId = issue.ProjectId;
			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId == currentUserId)
			{
				db.Comments.RemoveRange(issue.Comments);
				db.Issues.Remove(issue);
				db.SaveChanges();
			}
			return RedirectToAction("Index", new {projectId});
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