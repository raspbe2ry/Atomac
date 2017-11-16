using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;

[assembly: OwinStartupAttribute(typeof(VezbaSRLugic.Startup))]
namespace VezbaSRLugic
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //app.MapSignalR(new HubConfiguration()
            //{
            //    EnableJavaScriptProxies = false
            //});

            app.Map("/signalr", map =>
            {
                map.UseCors(CorsOptions.AllowAll);
                var hubConfiguration = new HubConfiguration
                {
                    // EnableJSONP = true;  // I am not using as CORS is working just fine
                    EnableJavaScriptProxies = false
                };
                map.RunSignalR(hubConfiguration);
            });
        }
    }
}