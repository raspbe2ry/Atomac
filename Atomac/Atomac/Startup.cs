using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Atomac.Startup))]
namespace Atomac
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
