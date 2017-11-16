using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Atomac_Sah_.Startup))]
namespace Atomac_Sah_
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
