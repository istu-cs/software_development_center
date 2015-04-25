using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Database.Common;
using Database.Entities;
using Database.Entities.Enum;
using Microsoft.AspNet.Identity;
using SDC.Web.Models;
using SDC.Web.Types;

namespace SDC.Web.Controllers
{
	public class TeamsController : BaseController
	{
		// GET: Teams
		public ActionResult Index(long? id)
		{
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			var team = db.Teams.Find(id);
			if (team == null || team.Type == TeamType.OneIssue)
			{
				return HttpNotFound();
			}

			if (team.Type == TeamType.Fictive)
			{
				var user = team.Performers.Single();
				return RedirectToAction("Index", "Users", new {id = user.Id});
			}

			var performers = team.Performers
				.Select(x => new UserListItemViewModel()
				{
					Id = x.Id,
					UserName = x.UserName
				})
				.ToList();

			var issues = team.IssueStatuses
				.Select(x => x.Issue)
				.Select(x => new IssueListItemViewModel()
				{
					Id = x.Id,
					Title = x.Title
				})
				.ToList();

			var model = new IndexTeamViewModel
			{
				Id = team.Id,
				Name = team.Name,
				Performers = performers,
				Issues = issues
			};
			return View(model);
		}

		[Authorize]
		public ActionResult Create()
		{
			var model = new CreateTeamViewModel();
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult Create(CreateTeamViewModel model)
		{
			if (model == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (!ModelState.IsValid || db.Teams.Any(x => x.Name == model.Name))
			{
				return View(model);
			}

			var user = db.Users.Find(User.Identity.GetUserId());
			var team = new Team()
			{
				Name = model.Name,
				Type = TeamType.Global,
				Performers = new[] {user}
			};

			db.Teams.Add(team);
			db.SaveChanges();

			return RedirectToAction("Index", new {id = team.Id});
		}

		[Authorize]
		public ActionResult Edit(long id)
		{
			var team = db.Teams.Find(id);
			if (team == null)
			{
				return HttpNotFound();
			}

			var performers = team.Performers
				.Select(x => new UserListItemViewModel()
				{
					Id = x.Id,
					UserName = x.UserName
				})
				.ToList();

			var model = new EditTeamViewModel()
			{
				Id = id,
				Name = team.Name,
				Performers = performers
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult Edit(EditTeamViewModel model)
		{
			if (model == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var team = db.Teams.Find(model.Id);
			if (team == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			team.Name = model.Name;
			db.SaveChanges();

			return RedirectToAction("Index", new {id = team.Id});
		}

		[Authorize]
		public ActionResult AddPerformer(long id)
		{
			if (!db.Teams.Any(x => x.Id == id))
			{
				return HttpNotFound();
			}

			var model = new AddPerformerTeamViewModel();
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult AddPerformer(long id, AddPerformerTeamViewModel model)
		{
			var team = db.Teams.Find(id);
			if (model == null || team == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (!ModelState.IsValid)
			{
				return RedirectToAction("Edit", new {id});
			}

			var performer = db.Users.Find(model.PerformerId);
			var constained = team.Performers.Any(x => x.Id == model.PerformerId);

			if (performer != null && !constained)
			{
				team.Performers.Add(performer);
				db.SaveChanges();
			}
			return RedirectToAction("Edit", new { id });
		}

		[Authorize]
		public ActionResult DeletePerformer(long id)
		{
			if (!db.Teams.Any(x => x.Id == id))
			{
				return HttpNotFound();
			}

			var model = new DeletePerformerTeamViewModel();
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult DeletePerformer(long id, DeletePerformerTeamViewModel model)
		{
			var team = db.Teams.Find(id);
			if (model == null || team == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (!ModelState.IsValid)
			{
				return RedirectToAction("Edit", new { id });
			}

			var performer = team.Performers.SingleOrDefault(x => x.Id == model.PerformerId);
			if (performer != null)
			{
				team.Performers.Remove(performer);
				db.SaveChanges();
			}
			return RedirectToAction("Edit", new { id });
		}

		[Authorize]
		public ActionResult ChangeCurrentTeam()
		{
			var user = db.Users.Find(User.Identity.GetUserId());
			var teams = user.Teams.Where(x => x.Type != TeamType.OneIssue);

			var model = new ChangeCurrentTeamViewModel
			{
				TeamId = User.GetCurrentTeamId(),
				Teams = new SelectList(teams, "Id", "Name")
			};
			return View(model);
		}

		[HttpPost]
		[Authorize]
		public ActionResult ChangeCurrentTeam(ChangeCurrentTeamViewModel model)
		{
			if (model == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = db.Users.Find(User.Identity.GetUserId());
			var team = user.Teams.SingleOrDefault(x => x.Id == model.TeamId);
			if (team == null)
			{
				return View(model);
			}

			MembershipTeam.SetCurrentTeam(team.Name);
			return RedirectToAction("Index", "Home");
		}
	}
}