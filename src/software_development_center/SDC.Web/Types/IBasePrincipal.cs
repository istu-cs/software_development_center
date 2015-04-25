using System.Security.Principal;

namespace SDC.Web.Types
{
	public interface IBasePrincipal : IPrincipal
	{
		long GetCurrentTeamId();
		string GetCurrentTeamName();
	}
}