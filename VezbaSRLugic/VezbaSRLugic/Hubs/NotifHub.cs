using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace VezbaSRLugic.Hubs
{
    public class NotifHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }
    }
}