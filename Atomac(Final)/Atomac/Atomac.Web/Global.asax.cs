using Atomac.EFDataLayer;
using Microsoft.AspNet.SignalR;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Atomac.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(200);
        }
    }
}
