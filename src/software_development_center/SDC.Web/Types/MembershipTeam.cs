using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Database.Common;
using Database.Entities;

namespace SDC.Web.Types
{
	public class MembershipTeam
	{
		private const string TeamCookieName = "user_team_name";

		public MembershipTeam(Team team)
		{
			TeamId = team.Id;
			TeamName = team.Name;
		}

		public object TeamId { get; private set; }
		public string TeamName { get; private set; }

		public static void SetCurrentTeam(string teamName)
		{
			var context = HttpContext.Current;
			var cookie = new HttpCookie(TeamCookieName, teamName);
			context.Response.Cookies.Set(cookie);
		}

		public static void ClearCurrentTeam()
		{
			var context = HttpContext.Current;
			if (context.Request.Cookies.AllKeys.Contains(TeamCookieName))
			{
				var cookie = context.Request.Cookies.Get(TeamCookieName);
				cookie.Expires = DateTime.Now.AddDays(-1);
				context.Response.Cookies.Set(cookie);
			}
		}

		public static MembershipTeam GetCurrentTeam()
		{
			var context = HttpContext.Current;
			var teamName = context.Request.Cookies.Get(TeamCookieName);
			if (teamName == null || string.IsNullOrEmpty(teamName.Value))
			{
				throw new Exception("Current team does not exist");
			}

			var db = SdcDbContext.Create();
			var team = db.Teams.SingleOrDefault(x => x.Name == teamName.Value);
			if (team == null)
			{
				return null;
			}

			return new MembershipTeam(team);
		}
	}
}