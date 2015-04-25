using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Database.Common;
using Database.Entities;
using Database.Entities.Enum;
using Microsoft.AspNet.Identity;
using SDC.Web.Extensions;
using SDC.Web.Extensions.Database;
using SDC.Web.Models;

namespace SDC.Web.Controllers
{
	public class IssuesController : BaseController
	{
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

			var dbIssues = db.Issues.Where(x => x.ProjectId == projectId).ToList();
			var issues = dbIssues
				.Select(x => x.ToModel())
				.ToList();

			ViewBag.ProjectId = projectId;
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
		public ActionResult Create(long? projectId, long? parentIssueId)
		{
			if (projectId == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			if (!db.Projects.Any(x => x.Id == projectId))
			{
				return HttpNotFound();
			}

			var model = new IssueViewModel
			{
				ProjectId = projectId.Value,
				ParentIssueId = parentIssueId
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Create(IssueViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var parentIssueExists = !model.ParentIssueId.HasValue || db.Issues.Any(x => x.Id == model.ParentIssueId);
			if (!parentIssueExists)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
		public ActionResult Edit(IssueViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var issue = db.Issues.Find(model.Id);
			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new {projectId = issue.ProjectId});
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
				return RedirectToAction("Index", new {projectId = issue.ProjectId});
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
				//db.Comments.RemoveRange(issue.Comments);
				db.Issues.Remove(issue);
				db.SaveChanges();
			}
			return RedirectToAction("Index", new {projectId});
		}

		[HttpPost]
		[Authorize]
		public ActionResult Assign(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var teamId = User.GetCurrentTeamId();
			var statusExists = db.IssueStatuses.Any(x => x.IssueId == issue.Id && x.TeamId == teamId);
			if (!statusExists)
			{
				var user = db.Users.Find(User.Identity.GetUserId());
				var team = user.Teams.Single(x => x.Id == teamId);
				AssignTeam(team, issue);
			}

			return RedirectToAction("Details", new {id = issue.Id});
		}

		[HttpPost]
		[Authorize]
		public ActionResult Unassign(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(id);
			if (issue == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var teamId = User.GetCurrentTeamId();
			var status = issue.IssueStatuses.SingleOrDefault(x => x.TeamId == teamId);
			if (status != null)
			{
				var team = db.Teams.Find(teamId);
				UnassignTeam(team, issue);
			}

			return RedirectToAction("Details", new {id = issue.Id});
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				db.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Helpers

		private void AssignTeam(Team team, Issue issue)
		{
			if (team == null || issue == null)
			{
				throw new ArgumentNullException();
			}

			if (issue.IssueStatuses.Any(x => x.TeamId == team.Id))
			{
				return;
			}

			foreach (var childIssue in issue.ChildIssues)
			{
				AssignTeam(team, childIssue);
			}

			var status = new IssueStatus
			{
				State = IssueState.Assigned,
				Issue = issue,
				Team = team
			};
			issue.IssueStatuses.Add(status);
			db.SaveChanges();
		}

		private void UnassignTeam(Team team, Issue issue)
		{
			if (team == null || issue == null)
			{
				throw new ArgumentNullException();
			}

			UnassignTeamDown(team, issue);
			if (issue.ParentIssue != null)
			{
				UnassignTeamUp(team, issue.ParentIssue);
			}
		}

		private void UnassignTeamDown(Team team, Issue issue)
		{
			if (team == null || issue == null)
			{
				throw new ArgumentNullException();
			}

			var status = issue.IssueStatuses.SingleOrDefault(x => x.TeamId == team.Id);
			if (status == null)
			{
				return;
			}

			db.Entry(status).State = EntityState.Deleted;
			db.SaveChanges();

			foreach (var childIssue in issue.ChildIssues)
			{
				UnassignTeam(team, childIssue);
			}
		}

		private void UnassignTeamUp(Team team, Issue issue)
		{
			if (team == null || issue == null)
			{
				throw new ArgumentNullException();
			}

			var status = issue.IssueStatuses.SingleOrDefault(x => x.TeamId == team.Id);
			while (status != null && issue != null)
			{
				db.Entry(status).State = EntityState.Deleted;
				issue = issue.ParentIssue;
				if (issue != null)
				{
					status = issue.IssueStatuses.SingleOrDefault(x => x.TeamId == team.Id);
				}
			}
			db.SaveChanges();
		}

		#endregion
	}
}