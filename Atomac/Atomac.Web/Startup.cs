using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Atomac.Web.Startup))]
namespace Atomac.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
