using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SDC.Web.Startup))]
namespace SDC.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
