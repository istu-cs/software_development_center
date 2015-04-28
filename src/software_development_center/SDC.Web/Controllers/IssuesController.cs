using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Database.Entities;
using Database.Entities.Enum;
using Microsoft.AspNet.Identity;
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

			if (db.Projects.All(x => x.Id != projectId))
			{
				return HttpNotFound();
			}

			var issues = db.Issues
				.Where(x => x.ProjectId == projectId)
				.Select(x => new IssueListItemViewModel
				{
					Id = x.Id,
					Title = x.Title
				});

			var model = new IndexIssueViewModel
			{
				ProjectId = projectId.Value,
				Issues = issues
			};

			return View(model);
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

			var childIssues = issue.ChildIssues
				.Select(x => new IssueListItemViewModel
				{
					Id = x.Id,
					Title = x.Title
				});

			var teamsProgress = issue.TeamsProgress
				.Select(x => new TeamProgressListItemViewModel
				{
					Status = x.Status,
					TeamId = x.TeamId,
					TeamName = x.Team.Name
				});

			var isAuthenticated = User.Identity.IsAuthenticated;
			var teamId = User.GetCurrentTeamId();
			var progress = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == teamId);

			var currentTeamCanAssign = false;
			var currentTeamCanStartProgress = false;
			var currentTeamCanSendToReview = false;
			var currentTeamCanUnassign = false;
			var currentTeamProgressStatus = progress == null ? TeamProgressStatus.Unoccupied : progress.Status;
			var currentUserCanEdit = issue.AuthorId == User.Identity.GetUserId();

			if (isAuthenticated && issue.Status == IssueStatus.Opened)
			{
				currentTeamCanAssign = currentTeamProgressStatus == TeamProgressStatus.Unoccupied;
				currentTeamCanUnassign = currentTeamProgressStatus != TeamProgressStatus.Unoccupied;
				currentTeamCanStartProgress = currentTeamProgressStatus == TeamProgressStatus.Assigned;
				currentTeamCanSendToReview = 
					currentTeamProgressStatus == TeamProgressStatus.InProgress || 
					currentTeamProgressStatus == TeamProgressStatus.PartiallyDone;
			}
			
			var model = new DetailsIssueViewModel()
			{
				Id = issue.Id,
				AuthorId = issue.AuthorId,
				AuthorName = issue.Author.UserName,
				Description = issue.Description,
				ParentIssueId = issue.ParentIssueId,
				ParentIssueTitle = issue.ParentIssue == null ? string.Empty : issue.ParentIssue.Title,
				ProjectId = issue.ProjectId,
				ProjectName = issue.Project.Name,
				Status = issue.Status,
				Title = issue.Title,
				ChildIssues = childIssues,
				TeamsProgress = teamsProgress,
				Type = issue.Type,
				CurrentTeamCanAssign = currentTeamCanAssign,
				CurrentTeamCanUnassign = currentTeamCanUnassign,
				CurrentTeamCanSendToReview = currentTeamCanSendToReview,
				CurrentTeamCanStartProgress = currentTeamCanStartProgress,
				CurrentTeamProgressStatus = currentTeamProgressStatus,
				CurrentUserCanEdit = currentUserCanEdit
			};

			return View(model);
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

			var types = Enum.GetNames(typeof(IssueType));

			var model = new CreateIssueViewModel
			{
				ProjectId = projectId.Value,
				ParentIssueId = parentIssueId,
				Types = new SelectList(types)
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Create(CreateIssueViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var types = Enum.GetNames(typeof(IssueType));
				model.Types = new SelectList(types);
				return View(model);
			}

			IssueType type;
			if (!Enum.TryParse(model.Type, out type))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var parentIssueExists = !model.ParentIssueId.HasValue || db.Issues.Any(x => x.Id == model.ParentIssueId);
			if (!parentIssueExists)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = new Issue
			{
				AuthorId = User.Identity.GetUserId(),
				Description = model.Description,
				ParentIssueId = model.ParentIssueId,
				ProjectId = model.ProjectId,
				Status = IssueStatus.Opened,
				Title = model.Title,
				Type = type
			};

			db.Issues.Add(issue);
			db.SaveChanges();

			if (issue.ParentIssueId.HasValue)
			{
				issue.ParentIssue = db.Issues.Find(issue.ParentIssueId);
				issue.ChildIssues = new Collection<Issue>();
				issue.TeamsProgress = new Collection<TeamProgress>();

				issue.ParentIssue.Status = IssueStatus.Waiting;
				db.SaveChanges();

				var teamId = User.GetCurrentTeamId();
				var parentIssue = issue.ParentIssue;
				var progress = parentIssue.TeamsProgress.SingleOrDefault(x => x.TeamId == teamId);
				if (progress == null || progress.Status == TeamProgressStatus.Unoccupied)
				{
					return RedirectToAction("Details", new { id = issue.Id });
				}

				if (progress.Status == TeamProgressStatus.Done)
				{
					progress.Status = TeamProgressStatus.PartiallyDone;
				}
				else if (progress.Status == TeamProgressStatus.ToVerify)
				{
					progress.Status = TeamProgressStatus.InProgress;
				}

				var team = db.Teams.Find(teamId);
				AssignTeam(team, issue);
			}

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

			var types = Enum.GetNames(typeof(IssueType));

			var model = new EditIssueViewModel
			{
				Id = issue.Id,
				Title = issue.Title,
				Description = issue.Description,
				ProjectId = issue.ProjectId,
				Type = issue.Type.ToString(),
				Types = new SelectList(types)
			};

			return View(model);
		}

		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public ActionResult Edit(EditIssueViewModel model)
		{
			if (!ModelState.IsValid)
			{
				var types = Enum.GetNames(typeof(IssueType));
				model.Types = new SelectList(types);
				return View(model);
			}

			IssueType type;
			if (!Enum.TryParse(model.Type, out type))
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var issue = db.Issues.Find(model.Id);
			var currentUserId = User.Identity.GetUserId();
			if (issue.AuthorId != currentUserId)
			{
				return RedirectToAction("Index", new {projectId = issue.ProjectId});
			}

			issue.Title = model.Title;
			issue.Description = model.Description;
			issue.Type = type;
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

			var model = new DeleteIssueViewModel
			{
				Id = issue.Id,
				Title = issue.Title,
				Description = issue.Description,
				AuthorName = issue.Author.UserName,
				ProjectName = issue.Project.Name
			};

			return View(model);
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
				var parentIssue = issue.ParentIssue;

				foreach (var childIssue in issue.ChildIssues)
				{
					childIssue.ParentIssueId = null;
				}

				db.Issues.Remove(issue);
				db.SaveChanges();

				if (parentIssue != null && parentIssue.ChildIssues.All(x => x.Status == IssueStatus.Closed))
				{
					parentIssue.Status = IssueStatus.Opened;
					db.SaveChanges();
				}
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
			var statusExists = db.TeamsProgress.Any(x => x.IssueId == issue.Id && x.TeamId == teamId);
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
			var status = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == teamId);
			if (status != null)
			{
				var team = db.Teams.Find(teamId);
				UnassignTeam(team, issue);
			}

			return RedirectToAction("Details", new {id = issue.Id});
		}

		[HttpPost]
		[Authorize]
		public ActionResult StartProgress(long? id)
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
			var progress = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == teamId);
			if (progress != null)
			{
				progress.Status = TeamProgressStatus.InProgress;
				db.SaveChanges();
			}

			return RedirectToAction("Details", new {id = issue.Id});
		}

		[HttpPost]
		[Authorize]
		public ActionResult SendToReview(long? id)
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
			var progress = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == teamId);
			if (progress != null)
			{
				progress.Status = TeamProgressStatus.ToVerify;
				db.SaveChanges();
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

			if (issue.TeamsProgress.Any(x => x.TeamId == team.Id))
			{
				return;
			}

			foreach (var childIssue in issue.ChildIssues)
			{
				AssignTeam(team, childIssue);
			}

			var status = new TeamProgress
			{
				Status = TeamProgressStatus.Assigned,
				Issue = issue,
				Team = team
			};
			issue.TeamsProgress.Add(status);
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

			var status = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == team.Id);
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

			var status = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == team.Id);
			while (status != null && issue != null)
			{
				db.Entry(status).State = EntityState.Deleted;
				issue = issue.ParentIssue;
				if (issue != null)
				{
					status = issue.TeamsProgress.SingleOrDefault(x => x.TeamId == team.Id);
				}
			}
			db.SaveChanges();
		}

		#endregion
	}
}