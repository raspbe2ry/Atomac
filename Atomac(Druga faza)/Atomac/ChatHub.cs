using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Atomac.Models;
using System.Threading.Tasks;
using Atomac.DTO;
using System.Web.Script.Serialization;

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
                        if (t.TeamMember.Email == teamMemberName)
                            lista.Add(t.Capiten.Email);
                        else
                            lista.Add(t.TeamMember.Email);
                        break; //samo 1 je aktivan-> ne moze vise
                    }
                }
                foreach (Team t in teamsPl2)
                {
                    if (t.Status == TStatus.Active)
                    {
                        t.Status = TStatus.Online;
                        if (t.TeamMember.Email == userName)
                            lista.Add(t.Capiten.Email);
                        else
                            lista.Add(t.TeamMember.Email);
                        break; //samo 1 je aktivan-> ne moze vise
                    }
                }
                Team team = dbContext.Teams.Where(p => (p.Capiten.Email == teamMemberName && p.TeamMember.Email == userName) ||
                                        (p.TeamMember.Email == teamMemberName && p.Capiten.Email == userName)).First();
                team.Status = TStatus.Active;
                dbContext.SaveChanges();
            }

            return Clients.Users(lista).ActivateTeam(result, teamName);
        }

        public Task SendGameRequest(string sndTeamName, string sndCptEmail,string rcvTeamName, string rcvCptEmail)
        {
            return Clients.User(rcvCptEmail).sendGameRequest(sndTeamName, sndCptEmail, rcvTeamName);
        }

        public Task ApproveGameRequest(string sndTeamName, string sndCptName, string oppTeamName, string result)
        {
            string oppCptName = Context.Request.User.Identity.Name;
            Team c1 = dbContext.Teams.Where(t => t.Capiten.Email == sndCptName && t.Name == sndTeamName && t.Status == TStatus.Active).ToList().First(); //tim izazivac
            Team c2 = dbContext.Teams.Where(t => t.Capiten.Email == oppCptName && t.Name == oppTeamName && t.Status == TStatus.Active).ToList().First(); //tim koji se izaziva
            int challenger = c1.Id; //id tima izazivaca
            int challenged = c2.Id; //id izazvanog tima
            if (result == "yes")
            {
                Game game = new Game();
                game.Team1Id = challenger;
                game.Team2Id = challenged;
                game.Team1 = c1;
                game.Team2 = c2;
                game.StatusT1 = GTStatus.Prepare;
                game.StatusT2 = GTStatus.Prepare;
                game.Date = DateTime.Now;
                game.Status = GStatus.Created;
                dbContext.Games.Add(game);

                c1.Status = TStatus.Busy;
                c2.Status = TStatus.Busy;
                dbContext.SaveChanges();

                DTOGame dGame = new DTOGame();
                dGame=dGame.GetById(game.Id);
                List<DTOAppUser> playersInfos = GetUsersInfos(challenger, challenged);
                List<string> playersEmails = new List<string>();
                foreach (DTOAppUser au in playersInfos)
                {
                    playersEmails.Add(au.Email);
                }
                var json = new JavaScriptSerializer().Serialize(dGame);
                return Clients.Users(playersEmails).GameCreationConfirmationApprove(json);
            }
            else
            {
                List<string> pE = new List<string>();
                pE.Add(sndCptName);
                pE.Add(oppCptName);
                return Clients.Users(pE).GameCreationConfirmationDisapprove();
            }
        }

        public List<DTOAppUser> GetUsersInfos(int id1, int id2)  //nije serverska funkcija, vec pomocna funkcija
        {
            List<Team> tList = dbContext.Teams.Where(t => t.Id == id1 || t.Id == id2).ToList();
            List<DTOAppUser> rList = new List<DTOAppUser>();
            foreach (Team t in tList)
            {
                DTOAppUser cap = new DTOAppUser();
                cap=cap.GetById(t.CapitenId);
                rList.Add(cap);
                DTOAppUser tm = new DTOAppUser();
                tm=tm.GetById(t.TeamMemberId);
                rList.Add(tm);
            }
            return rList;
        }

        public Task SendMessage(string nick, string message, string gameId) //poruke tokom kreiranje igre i partije, chat je tranzijentni
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).SendGameMessage(nick, message);
        }

        public List<string> EmailsFromPlayersInGame(int gameId)
        {
            Game game = dbContext.Games.Find(gameId);
            List<Team> tList = dbContext.Teams.Where(t => t.Id == game.Team1Id || t.Id == game.Team2Id).ToList();
            List<string> rList = new List<string>();
            foreach (Team t in tList)
            {
                rList.Add(t.Capiten.Email);
                rList.Add(t.TeamMember.Email);
            }
            return rList;
        }

        public Task SendDroppedCheck(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedCheck(value, senderTeamId);
        }

        public Task SendDroppedCheckMate(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedCheckMate(value, senderTeamId);
        }

        public Task SendDroppedPawnOnFirstLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedPawnOnFirstLine(value, senderTeamId);
        }

        public Task SendDroppedPawnOnLastLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedPawnOnLastLine(value, senderTeamId);
        }

        public Task SendDroppedFigureOnLastLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedFigureOnLastLine(value, senderTeamId);
        }

        public Task SendGameDuration(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGameDuration(value, senderTeamId);
        }

        public Task SendGamePoints(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGamePoints(value, senderTeamId);
        }

        public Task SendGameTokens(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(Int32.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGameTokens(value, senderTeamId);
        }
    }
}