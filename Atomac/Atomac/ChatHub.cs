using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Atomac.Models;
using System.Threading.Tasks;

namespace Atomac.Controllers
{
    public class ChatHub : Hub
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();

        public Task Send(string nick, string message)
        {
            string userName = Context.Request.User.Identity.Name;

            Message mes = new Message();
            mes.LinkTag = false;
            ApplicationUser us=dbContext.Users.Where(b => b.NickName == nick).FirstOrDefault();
            mes.SenderId = us.Id;
            mes.Text = message;
            mes.Time = DateTime.Now;
            dbContext.Messages.Add(mes);
            dbContext.SaveChanges();

             return Clients.All.addNewMessageToPage(nick, message);
        }

        public Task SendTeamRequest(string receiverMail, string teamName)
        {
            string userName = Context.Request.User.Identity.Name;

            return Clients.User(receiverMail).sendTeamRequest(userName, teamName);
        }

        public Task ApproveTeamRequest(string teamMemberName, string teamName, string result)
        {
            string userName = Context.Request.User.Identity.Name;

            List<string> lista = new List<String>();
            lista.Add(teamMemberName);
            lista.Add(userName);

            if (result == "yes")
            {
                Team team = new Team();
                ApplicationUser capiten = dbContext.Users.Where(p => p.Email == teamMemberName).First();
                ApplicationUser player2 = dbContext.Users.Where(p => p.Email == userName).First();
                team.Capiten = capiten;
                team.TeamMember = player2;
                team.Name = teamName;
                team.Points = 0;
                team.Status = TStatus.Active;
                capiten.Status = PStatus.Active;
                player2.Status = PStatus.Active;
                capiten.AdminedTeams.Add(team);
                player2.Teams.Add(team);
                dbContext.SaveChanges();
            }

            return Clients.Users(lista).ActivateTeam(result, teamName);
        }
    }
}