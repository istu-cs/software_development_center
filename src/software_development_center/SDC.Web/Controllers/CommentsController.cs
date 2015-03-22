using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Routing;
using Database.Common;
using Microsoft.AspNet.Identity;
using SDC.Web.Extensions;
using SDC.Web.Extensions.Database;
using SDC.Web.Models;

namespace SDC.Web.Controllers
{
	public class CommentsController : Controller
	{
		private SdcDbContext db = SdcDbContext.Create();

		public ActionResult Index(long? issueId)
		{
			if (issueId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			
			var issue = db.Issues.Find(issueId);
			var comments = db.Comments
				.Where(x => x.IssueId == issueId).ToList()
				.Select(x => x.ToModel());

			ViewBag.IssueId = issueId;
			ViewBag.ProjectId = issue.ProjectId;
			return View(comments.ToList());
		}

		public ActionResult Details(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var comment = db.Comments.Find(id);
			if (comment == null)
			{
				return HttpNotFound();
			}
			return View(comment.ToModel());
		}

		[Authorize]
		public ActionResult Create(long? issueId)
		{
			if (issueId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var model = new CommentModel
			{
				IssueId = issueId.Value
			};

			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CommentModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var comment = model.ToDbModel();
			comment.AuthorId = User.Identity.GetUserId();
			comment.Time = DateTime.Now;

			db.Comments.Add(comment);
			db.SaveChanges();

			return RedirectToAction("Details", new {id = comment.Id});
		}

		[Authorize]
		public ActionResult Edit(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var comment = db.Comments.Find(id);
			if (comment == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (comment.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new { issueId = comment.IssueId });
			}

			return View(comment.ToModel());
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(CommentModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var comment = db.Comments.Find(model.Id);
			var currentUserId = User.Identity.GetUserId();

			if (comment.AuthorId == currentUserId)
			{
				return RedirectToAction("Index", new { issueId = comment.IssueId });
			}

			comment.Text = model.Text;
			db.SaveChanges();

			return RedirectToAction("Details", new {comment.Id});
		}

		[Authorize]
		public ActionResult Delete(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var comment = db.Comments.Find(id);
			if (comment == null)
			{
				return HttpNotFound();
			}

			var currentUserId = User.Identity.GetUserId();
			if (comment.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new { issueId = comment.IssueId });
			}

			return View(comment.ToModel());
		}

		[HttpPost, ActionName("Delete")]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(long id)
		{
			var comment = db.Comments.Find(id);
			var currentUserId = User.Identity.GetUserId();
			if (comment.AuthorId == currentUserId)
			{
				db.Comments.Remove(comment);
				db.SaveChanges();
			}
			return RedirectToAction("Index", new {issueId = comment.IssueId});
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