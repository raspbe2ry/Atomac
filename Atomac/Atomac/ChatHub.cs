using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Atomac.Models;

namespace Atomac.Controllers
{
    public class ChatHub : Hub
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        public void Send(string nick, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Message mes = new Message();
            mes.LinkTag = false;
            ApplicationUser us=dbContext.Users.Where(b => b.NickName == nick).FirstOrDefault();
            mes.SenderId = us.Id;
            mes.Text = message;
            mes.Time = DateTime.Now;
            dbContext.Messages.Add(mes);
            dbContext.SaveChanges();

            Clients.All.addNewMessageToPage(nick, message);
        }
    }
}