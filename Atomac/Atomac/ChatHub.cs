using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Atomac.Controllers
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            String l = Context.ConnectionId;
            Clients.All.addNewMessageToPage(name, message);
           // int a = 5;
        }
    }
}