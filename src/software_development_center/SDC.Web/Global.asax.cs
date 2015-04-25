using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using SDC.Web.Types;

namespace SDC.Web
{
	public class MvcApplication : HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
		{
			if (HttpContext.Current.User.Identity.IsAuthenticated)
			{
				HttpContext.Current.User = new BasePrincipal(HttpContext.Current.User);
			}
		}
	}
}