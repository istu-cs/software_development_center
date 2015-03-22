using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SDC.Web.Startup))]
namespace SDC.Web
{
    public class Startup : Database.Common.Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
