using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Atomac.Models;
using System.Threading.Tasks;
using Atomac.DTO;

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
                team.Status = TStatus.Online;
                capiten.AdminedTeams.Add(team);
                player2.Teams.Add(team);
                dbContext.SaveChanges();
            }

            return Clients.Users(lista).MakeTeam(result, teamName);
        }

        public Task SendActivateTeamRequest(string captainMail, string teamMemberMail, string teamName)
        {
            string userName = Context.Request.User.Identity.Name;
            if(userName == captainMail)
            {
                //mi smo kapiten pa saljemo zahtev drugom clanu tima
                return Clients.User(teamMemberMail).sendActivateTeamRequest(userName, teamName);
            }
            else
            {
                return Clients.User(captainMail).sendActivateTeamRequest(userName, teamName);
            }
        }

        public Task ApproveActivateTeamRequest(string teamMemberName, string teamName, string result)
        {
            string userName = Context.Request.User.Identity.Name;

            List<string> lista = new List<String>();
            lista.Add(teamMemberName);
            lista.Add(userName);

            if (result == "yes")
            {
                List<Team> teamsPl1 = dbContext.Teams.Where(p => (p.Capiten.Email == teamMemberName || p.TeamMember.Email == teamMemberName)).ToList();
                List<Team> teamsPl2 = dbContext.Teams.Where(p => (p.Capiten.Email == userName || p.TeamMember.Email == userName)).ToList();
                foreach (Team t in teamsPl1)
                {
                    if (t.Status == TStatus.Active)
                    {
                        t.Status = TStatus.Online;
                        break; //samo 1 je aktivan-> ne moze vise
                    }
                }
                foreach (Team t in teamsPl2)
                {
                    if (t.Status == TStatus.Active)
                    {
                        t.Status = TStatus.Online;
                        break; //samo 1 je aktivan-> ne moze vise
                    }
                }
                Team team = dbContext.Teams.Where(p => (p.Capiten.Email == teamMemberName && p.TeamMember.Email == userName) ||
                                        (p.TeamMember.Email == teamMemberName && p.TeamMember.Email == userName)).First();
                team.Status = TStatus.Active;
                dbContext.SaveChanges();
            }

            return Clients.Users(lista).ActivateTeam(result, teamName);
        }

        public Task SendGameRequest(string sndTeamName, string sndCptEmail,string rcvTeamName, string rcvCptEmail)
        {
            return Clients.User(rcvCptEmail).sendGameRequest(sndTeamName, sndCptEmail, rcvTeamName);
        }

        public Task ApproveGameRequest(string sndTeamName, string sndCptName,string oppTeamName, string result)
        {
        }
    }
}