using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using Database.Common;
using Database.Entities;
using Microsoft.AspNet.Identity;

namespace SDC.Web.Types
{
	public class BasePrincipal : IBasePrincipal
	{
		private readonly MembershipTeam team;
		private readonly IPrincipal principal;

		public BasePrincipal(IPrincipal principal)
		{
			team = MembershipTeam.GetCurrentTeam();
			this.principal = principal;
			Identity = principal.Identity;
		}

		public IIdentity Identity { get; private set; }

		public bool IsInRole(string role)
		{
			return principal.IsInRole(role);
		}

		public long GetCurrentTeamId()
		{
			return (long)team.TeamId;
		}

		public string GetCurrentTeamName()
		{
			return team == null ? string.Empty : team.TeamName;
		}
	}
}