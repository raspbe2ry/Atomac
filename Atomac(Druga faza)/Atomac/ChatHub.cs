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
using RedisDataLayer;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace Atomac.Controllers
{
    public class ChatHub : Hub
    {
        ApplicationDbContext dbContext = new ApplicationDbContext();
        RedisFunctions rf = new RedisFunctions();

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

                AddGameToRedisDB(game);

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

        public Task SubmitChanges(DTOGameMini gm)
        {
            //DTOGameMini gm = new DTOGameMini();
            //MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            //DataContractJsonSerializer ser = new DataContractJsonSerializer(gm.GetType());
            //gm = ser.ReadObject(ms) as DTOGameMini;
            //ms.Close();

            int res =  AddSubmitedGameChangesToRedisDB(gm);
            //treba da se vidi status da li da se redirektujemo ili da se vrati stranica
            if(res==1)
            {
                //poslati svima submit prvog
            }
            else if(res==2)
            {
                //poslati svima da je sve ok->prelazak na igru
            }
            else if(res==0)
            {
                //poslati svima da se ne poklapaju pravila i vracaju se u prepare stanje
            }
            //ovo cisto zbog return
            return Clients.All.NekaFunkcija("");
        }

        public Task StartGame(string json)
        {
            DTOGameMini gm = new DTOGameMini();
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(gm.GetType());
            gm = ser.ReadObject(ms) as DTOGameMini;
            ms.Close();

            DeleteGameFromRedisDB(gm);
            //ovde posle ide redirekcija
            return Clients.All.NekaFunkcija("");
        }

        private void DeleteGameFromRedisDB(DTOGameMini g)
        {
            string hashGame = rf.MakeHashId("game", g.Id.ToString());
            string keyT1 = rf.GetHashAttributeValue(hashGame, "t1");
            string keyT2 = rf.GetHashAttributeValue(hashGame, "t2");
            rf.DeleteKey(hashGame);
            DeleteTeam(keyT1);
            DeleteTeam(keyT2);
        }

        private void DeleteTeam(string keyT)
        {
            string rules = rf.GetHashAttributeValue(keyT, "rules");
            rf.DeleteKey(keyT);
            rf.DeleteKey(rules);
        }

        private void AddGameToRedisDB(Game g)
        {
            string hashGame = rf.MakeHashId("game", g.Id.ToString());
            if (!rf.CheckIfKeyExists(hashGame))
            {
                string keyT1 = rf.MakeHashId("team", g.Team1Id.ToString());
                string keyT2 = rf.MakeHashId("team", g.Team2Id.ToString());
                rf.CreateGameHash(hashGame, keyT1, keyT2);
                rf.CreateTeamHash(hashGame, keyT1);
                rf.CreateTeamHash(hashGame, keyT2);
            }
        }

        //cuvamo tim dok igra, tako da je jedinstven uvek team:id
        //hashTeam je team koji vrsi submit trenutno
        //return: 
        //1->prvi tim je izvrsio submit; 
        //2->drugi tim je izvrsio submit i pravila se poklapaju; 
        //0->drugi tim je izvrsio submit, pravila se ne poklapaju pa su oba tima vracena u prepare stanje(pravila su i dalje selektovana)
        private int AddSubmitedGameChangesToRedisDB(DTOGameMini gm)
        {
            string hashTeam = rf.MakeHashId("team", gm.TeamId);
            string hashGame = rf.MakeHashId("game", gm.Id);
            string team1 = rf.GetHashAttributeValue(hashGame, "t1");
            string team2 = rf.GetHashAttributeValue(hashGame, "t2");
            string statusT1 = rf.GetHashAttributeValue(team1, "status");
            string statusT2 = rf.GetHashAttributeValue(team2, "status");
            rf.SetHashAttributeValue(hashTeam, "status", "ready");
            if (String.Equals(statusT1, statusT2))
            {
                DeleteSetOfRules(hashTeam);
                SubmitFirstSetOfRules(gm, hashTeam);
                return 1;
            }
            else
            {
                bool res = SubmitSecondSetOfRules(gm, hashTeam, team1, team2);
                if (res)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        private void SubmitFirstSetOfRules(DTOGameMini gm, string hashTeam)
        {
            string listOfRules = rf.GetHashAttributeValue(hashTeam, "rules");
            rf.PushItemToList(listOfRules, gm.Points);
            rf.PushItemToList(listOfRules, gm.Tokens);
            rf.PushItemToList(listOfRules, gm.Duration);
            rf.PushItemToList(listOfRules, gm.DroppedCheck);
            rf.PushItemToList(listOfRules, gm.DroppedCheckMate);
            rf.PushItemToList(listOfRules, gm.DroppedPawnOnFirstLine);
            rf.PushItemToList(listOfRules, gm.DroppedPawnOnLastLine);
            rf.PushItemToList(listOfRules, gm.DroppedFigureOnLastLine);
        }
        //neko je vec submitovao
        private bool SubmitSecondSetOfRules(DTOGameMini gm, string hashTeam, string team1, string team2)
        {
            DeleteSetOfRules(hashTeam);
            SubmitFirstSetOfRules(gm, hashTeam);
            bool res = false;
            string hashGame = rf.MakeHashId("game", gm.Id);
            string hashTeam1 = rf.MakeHashId("team", team1);
            string hashTeam2 = rf.MakeHashId("team", team2);
            string rules1 = rf.MakeHashId(rf.MakeHashId(hashGame, hashTeam1), "rules");
            string rules2 = rf.MakeHashId(rf.MakeHashId(hashGame, hashTeam2), "rules");
            res = CheckRules(rules1, rules2);
            if (!res)
            {
                rf.SetHashAttributeValue(team1, "status", "prepare");
                rf.SetHashAttributeValue(team2, "status", "prepare");
            }
            return res;
        }

        private bool CheckRules(string team1, string team2)
        {
            int length = (int)rf.GetListCount(team1);
            for(int i=0; i<length; i++)
            {
                string ruleT1 = rf.GetItemFromList(team1, i);
                string ruleT2 = rf.GetItemFromList(team2, i);
                if(!String.Equals(ruleT1,ruleT2))
                {
                    return false;
                }
            }
            return true;
        }

        private void DeleteSetOfRules(string hashTeam)
        {
            string listOfRules = rf.GetHashAttributeValue(hashTeam, "rules");
            if(rf.CheckIfKeyExists(listOfRules))
                rf.RemoveAllFromList(listOfRules);
        }
    }
}