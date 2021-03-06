﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Atomac.EFDataLayer.Models;
using Atomac.EFDataLayer;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Atomac.RedisDataLayer;
using Atomac.EFDataLayer.DTO;
using System.Web.Script.Serialization;

namespace Atomac.Web
{
    public class ChatHub : Hub
    {

        ApplicationDbContext dbContext = new ApplicationDbContext();
        RedisController rc = new RedisController();

        public Task Send(string nick, string message)
        {
            string userName = Context.Request.User.Identity.Name;

            Message mes = new Message();
            mes.LinkTag = false;
            ApplicationUser us = dbContext.Users.Where(b => b.NickName == nick).FirstOrDefault();
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
                team.Points = 1500;
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
            if (userName == captainMail)
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

            //return Clients.Users(lista).ActivateTeam(result, teamName);
            return Clients.All.ActivateTeam(result);
        }

        public Task SendGameRequest(string sndTeamName, string sndCptEmail, string rcvTeamName, string rcvCptEmail)
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
                //ovo da li treba?
                c1.GamesAsFirst.Add(game);
                c2.GamesAsSecond.Add(game);
                c2.Status = TStatus.Busy;
                dbContext.SaveChanges();

                AddGameToRedisDB(game);

                DTOGame dGame = new DTOGame();
                dGame = dGame.GetById(game.Id);
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
                cap = cap.GetById(t.CapitenId);
                rList.Add(cap);
                DTOAppUser tm = new DTOAppUser();
                tm = tm.GetById(t.TeamMemberId);
                rList.Add(tm);
            }
            return rList;
        }

        public Task SendMessageInGame(string nick, string message, string gameId, string teamId) //poruke tokom kreiranje igre i partije, chat je tranzijentni
        {
            string hashGame = rc.MakeHashId("game", gameId);
            string messageList = rc.GetHashAttributeValue(hashGame, "msg");
            string messageValue = nick + ": " + message;
            rc.PushItemToList(messageList, messageValue);

            string team = "team:" + teamId;
            string color = "";
            if (team.Equals(rc.GetHashAttributeValue(hashGame, "t1")))
            {
                color = "blue";
            }
            else color = "red";

            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).AddNewMessageToGamePane(nick, message, color);
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
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedCheck(value, senderTeamId);
        }

        public Task SendDroppedCheckMate(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedCheckMate(value, senderTeamId);
        }

        public Task SendDroppedPawnOnFirstLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedPawnOnFirstLine(value, senderTeamId);
        }

        public Task SendDroppedPawnOnLastLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedPawnOnLastLine(value, senderTeamId);
        }

        public Task SendDroppedFigureOnLastLine(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnDroppedFigureOnLastLine(value, senderTeamId);
        }

        public Task SendGameDuration(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGameDuration(value, senderTeamId);
        }

        public Task SendGamePoints(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGamePoints(value, senderTeamId);
        }

        public Task SendGameTokens(string value, string senderTeamId, string gameId)
        {
            List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gameId));
            return Clients.Users(playersEmails).ReturnGameTokens(value, senderTeamId);
        }

        private List<string> GetPlayersEmailsForTeam(int teamId)
        {
            Team team = dbContext.Teams.Find(teamId);
            List<string> rList = new List<string>();
            rList.Add(team.Capiten.Email);
            rList.Add(team.TeamMember.Email);
            return rList;
        }

        public Task SubmitChanges(DTOGameMini gm)
        {
            int res = AddSubmitedGameChangesToRedisDB(gm);
            //treba da se vidi status da li da se redirektujemo ili da se vrati stranica
            if (res == 1)
            {
                //poslati svima submit prvog
                List<string> playersEmails = GetPlayersEmailsForTeam(int.Parse(gm.TeamId));
                return Clients.Users(playersEmails).SendSubmitOfOne();
            }
            else if (res == 2)
            {
                //poslati svima da je sve ok->prelazak na igru
                //dodati pravila iz redisa u bazu(staviti i status igre->to mozda u start game)

                string hashGame = rc.MakeHashId("game", gm.Id);
                string team1 = rc.GetHashAttributeValue(hashGame, "t1");
                string rules = rc.MakeHashId(rc.MakeHashId(hashGame, team1), "rules");

                Game g = dbContext.Games.Find(int.Parse(gm.Id));
                g.Duration = int.Parse(gm.Duration);
                g.Tokens = int.Parse(gm.Tokens);

                Rules r = new Rules();
                if (gm.DroppedPawnOnLastLine.Equals("yes"))
                    r.DroppedPawnOnLastLine = true;
                else r.DroppedPawnOnLastLine = false;

                if (gm.DroppedPawnOnFirstLine.Equals("yes"))
                    r.DroppedPawnOnFirstLine = true;
                else r.DroppedPawnOnFirstLine = false;

                if (gm.DroppedFigureOnLastLine.Equals("yes"))
                    r.DroppedFigureOnLastLine = true;
                else r.DroppedFigureOnLastLine = false;

                if (gm.DroppedCheckMate.Equals("yes"))
                    r.DroppedCheckMate = true;
                else r.DroppedCheckMate = false;

                if (gm.DroppedCheck.Equals("yes"))
                    r.DroppedCheck = true;
                else r.DroppedCheck = false;

                dbContext.Ruless.Add(r);
                g.Rules = r;
                dbContext.SaveChanges();

                List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gm.Id));
                DTOGame dGame = new DTOGame();
                dGame = dGame.GetById(int.Parse(gm.Id));
                return Clients.Users(playersEmails).SendStartGame(dGame);
            }
            else//res=0
            {
                //poslati svima da se ne poklapaju pravila i vracaju se u prepare stanje
                List<string> playersEmails = EmailsFromPlayersInGame(int.Parse(gm.Id));
                return Clients.Users(playersEmails).ResetControl();
            }
        }

        //metoda se ne zove jer se ne prelazi na drugi view
        public Task StartGame(DTOGameMini gm)
        {
            //mozda staviti status igre ako nije vec
            DeleteGameFromRedisDB(gm);
            //ovde posle ide redirekcija
            return Clients.All.NekaFunkcija("");
        }

        private void DeleteGameFromRedisDB(DTOGameMini g)
        {
            string hashGame = rc.MakeHashId("game", g.Id.ToString());
            string keyT1 = rc.GetHashAttributeValue(hashGame, "t1");
            string keyT2 = rc.GetHashAttributeValue(hashGame, "t2");
            rc.DeleteKey(hashGame);
            DeleteTeam(keyT1);
            DeleteTeam(keyT2);
        }

        private void DeleteTeam(string keyT)
        {
            string rules = rc.GetHashAttributeValue(keyT, "rules");
            rc.DeleteKey(keyT);
            rc.DeleteKey(rules);
        }

        private void AddGameToRedisDB(Game g)
        {
            string hashGame = rc.MakeHashId("game", g.Id.ToString());
            if (!rc.CheckIfKeyExists(hashGame))
            {
                string keyT1 = rc.MakeHashId("team", g.Team1Id.ToString());
                string keyT2 = rc.MakeHashId("team", g.Team2Id.ToString());
                string keyMessages = rc.MakeHashId(hashGame, "messages");
                rc.CreateGameHash(hashGame, keyT1, keyT2, keyMessages);
                rc.CreateTeamHash(hashGame, keyT1);
                rc.CreateTeamHash(hashGame, keyT2);
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
            string hashTeam = rc.MakeHashId("team", gm.TeamId);
            string hashGame = rc.MakeHashId("game", gm.Id);
            string team1 = rc.GetHashAttributeValue(hashGame, "t1");
            string team2 = rc.GetHashAttributeValue(hashGame, "t2");
            string statusT1 = rc.GetHashAttributeValue(team1, "status");
            string statusT2 = rc.GetHashAttributeValue(team2, "status");
            rc.SetHashAttributeValue(hashTeam, "status", "ready");
            if (String.Equals(statusT1, statusT2))
            {
                DeleteSetOfRules(hashTeam);
                SubmitFirstSetOfRules(gm, hashTeam);
                return 1;
            }
            else
            {
                bool res = SubmitSecondSetOfRules(gm, hashTeam, team1, team2);
                /*
                if (res)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
                */
                return (res) ? 2 : 0;
            }
        }

        private void SubmitFirstSetOfRules(DTOGameMini gm, string hashTeam)
        {
            string listOfRules = rc.GetHashAttributeValue(hashTeam, "rules");
            rc.PushItemToList(listOfRules, gm.Tokens);
            rc.PushItemToList(listOfRules, gm.Duration);
            rc.PushItemToList(listOfRules, gm.DroppedCheck);
            rc.PushItemToList(listOfRules, gm.DroppedCheckMate);
            rc.PushItemToList(listOfRules, gm.DroppedPawnOnFirstLine);
            rc.PushItemToList(listOfRules, gm.DroppedPawnOnLastLine);
            rc.PushItemToList(listOfRules, gm.DroppedFigureOnLastLine);
        }

        //neko je vec submitovao
        private bool SubmitSecondSetOfRules(DTOGameMini gm, string hashTeam, string team1, string team2)
        {
            DeleteSetOfRules(hashTeam);
            SubmitFirstSetOfRules(gm, hashTeam);
            bool res = false;
            string hashGame = rc.MakeHashId("game", gm.Id);
            string rules1 = rc.MakeHashId(rc.MakeHashId(hashGame, team1), "rules");
            string rules2 = rc.MakeHashId(rc.MakeHashId(hashGame, team2), "rules");
            res = CheckRules(rules1, rules2);
            if (!res)
            {
                rc.SetHashAttributeValue(team1, "status", "prepare");
                rc.SetHashAttributeValue(team2, "status", "prepare");
            }
            return res;
        }

        private bool CheckRules(string team1, string team2)
        {
            int length = (int)rc.GetListCount(team1);
            for (int i = 0; i < length; i++)
            {
                string ruleT1 = rc.GetItemFromList(team1, i);
                string ruleT2 = rc.GetItemFromList(team2, i);
                if (!string.Equals(ruleT1, ruleT2))
                {
                    return false;
                }
            }
            return true;
        }

        private void DeleteSetOfRules(string hashTeam)
        {
            string listOfRules = rc.GetHashAttributeValue(hashTeam, "rules");
            if (rc.CheckIfKeyExists(listOfRules))
                rc.RemoveAllFromList(listOfRules);
        }

        public Task MoveFigure(string move)
        {
            string userEmail = Context.Request.User.Identity.Name;
            JObject jObject = JObject.Parse(move);
            JToken d = jObject;
            Move m = new Move();
            m.Captured = (string)d["Captured"];
            m.Color = (string)d["Color"];
            m.GameId = int.Parse((string)d["GameId"]);
            m.State = (string)d["State"];
            m.Black = d["Black"].ToObject<List<string>>();
            m.White = d["White"].ToObject<List<string>>();
            m.Board = (Board)int.Parse((string)d["Board"]);
            Game g = dbContext.Games.Where(x => x.Id == m.GameId).First();
            m.Game = g;
            g.Moves.Add(m);
            dbContext.Moves.Add(m);
            dbContext.SaveChanges();
            string teamId = "";
            if (userEmail.Equals(g.Team1.Capiten.Email) || userEmail.Equals(g.Team1.TeamMember.Email))
                teamId = g.Team1.Id.ToString();
            else teamId = g.Team2.Id.ToString();

            List<string> playersEmails = EmailsFromPlayersInGame(g.Id);
            return Clients.Users(playersEmails).MoveFigureOnTable(move, userEmail, teamId);
        }

        public Task MoveFigureAndFinishGame(string move, string poruka)
        {
            string userEmail = Context.Request.User.Identity.Name;
            JObject jObject = JObject.Parse(move);
            JToken d = jObject;
            Move m = new Move();
            m.Captured = (string)d["Captured"];
            m.Color = (string)d["Color"];
            m.GameId = int.Parse((string)d["GameId"]);
            m.State = (string)d["State"];
            m.Black = d["Black"].ToObject<List<string>>();
            m.White = d["White"].ToObject<List<string>>();
            m.Board = (Board)int.Parse((string)d["Board"]);
            Game g = dbContext.Games.Where(x => x.Id == m.GameId).First();
            m.Game = g;
            g.Moves.Add(m);
            dbContext.Moves.Add(m);
            dbContext.SaveChanges();

            string teamId = FinishGame(g, userEmail, poruka, false);

            List<string> playersEmails = EmailsFromPlayersInGame(g.Id);
            return Clients.Users(playersEmails).MoveFigureAndFinishGame(move, userEmail, teamId, poruka);
        }

        public Task TimeOutAndFinishGame(string gameId, string poruka)
        {
            string userEmail = Context.Request.User.Identity.Name;
            int gId = int.Parse(gameId);
            Game g = dbContext.Games.Where(x => x.Id == gId).First();

            string teamId = FinishGame(g, userEmail, poruka, true);

            List<string> playersEmails = EmailsFromPlayersInGame(g.Id);
            return Clients.Users(playersEmails).TimeOutAndFinishGame(teamId);

        }

        //ciji je gm.TeamId je pobedio (f-ja se poziva kad neko napravi mat)
        private string FinishGame(Game g, string finisher, string poruka, bool timeOut)
        {
            g.Status = GStatus.Finished;
            int rnewT1 = 0;
            int rnewT2 = 0;
            int rnewP11 = 0;
            int rnewP12 = 0;
            int rnewP21 = 0;
            int rnewP22 = 0;
            double w = 0.5;
            string teamId;
            if (poruka.Equals("Checkmate"))
                w = 1.0;
            bool winner = true;
            if (timeOut)
                winner = false;
            if (finisher.Equals(g.Team1.Capiten.Email) || finisher.Equals(g.Team1.TeamMember.Email))
            {
                rnewT1 = CountNewRanking(winner, g.Team1.Points, g.Team2.Points, w);
                rnewP11 = CountNewRanking(winner, g.Team1.Capiten.Points, g.Team2.Capiten.Points, w);
                rnewP12 = CountNewRanking(winner, g.Team1.TeamMember.Points, g.Team2.TeamMember.Points, w);

                rnewT2 = CountNewRanking(!winner, g.Team1.Points, g.Team2.Points, w);
                rnewP21 = CountNewRanking(!winner, g.Team1.Capiten.Points, g.Team2.Capiten.Points, w);
                rnewP22 = CountNewRanking(!winner, g.Team1.TeamMember.Points, g.Team2.TeamMember.Points, w);

                SetTitle(rnewP11, g.Team1.Capiten);
                SetTitle(rnewP12, g.Team1.TeamMember);
                SetTitle(rnewP21, g.Team2.Capiten);
                SetTitle(rnewP22, g.Team2.TeamMember);

                if (w == 0.5)
                {
                    teamId = g.Team1.Id.ToString(); //u sustini je nevazno koji tim jer jer je remi, samo je poruka bitna. Ali ipak postavljeno
                    g.Team1.Capiten.Draws++;
                    g.Team1.TeamMember.Draws++;
                    g.Team2.Capiten.Draws++;
                    g.Team2.TeamMember.Draws++;
                    g.StatusT1 = GTStatus.Draw;
                    g.StatusT2 = GTStatus.Draw;
                }
                else
                {
                    if (winner)
                    {
                        teamId = g.Team1.Id.ToString();
                        g.Team1.Capiten.Wins++;
                        g.Team1.TeamMember.Wins++;
                        g.Team2.Capiten.Losses++;
                        g.Team2.TeamMember.Losses++;
                        g.StatusT1 = GTStatus.Winner;
                        g.StatusT2 = GTStatus.Losser;
                        g.Team1.Capiten.Tokens += g.Tokens;
                        g.Team1.TeamMember.Tokens += g.Tokens;
                        g.Team2.Capiten.Tokens -= g.Tokens;
                        g.Team2.TeamMember.Tokens -= g.Tokens;
                    }
                    else
                    {
                        teamId = g.Team2.Id.ToString();
                        g.Team1.Capiten.Losses++;
                        g.Team1.TeamMember.Losses++;
                        g.Team2.Capiten.Wins++;
                        g.Team2.TeamMember.Wins++;
                        g.StatusT2 = GTStatus.Winner;
                        g.StatusT1 = GTStatus.Losser;
                        g.Team2.Capiten.Tokens += g.Tokens;
                        g.Team2.TeamMember.Tokens += g.Tokens;
                        g.Team1.Capiten.Tokens -= g.Tokens;
                        g.Team1.TeamMember.Tokens -= g.Tokens;
                    }
                }
            }
            else
            {
                rnewT1 = CountNewRanking(!winner, g.Team1.Points, g.Team2.Points, w);
                rnewP11 = CountNewRanking(!winner, g.Team1.Capiten.Points, g.Team2.Capiten.Points, w);
                rnewP12 = CountNewRanking(!winner, g.Team1.TeamMember.Points, g.Team2.TeamMember.Points, w);

                rnewT2 = CountNewRanking(winner, g.Team1.Points, g.Team2.Points, w);
                rnewP21 = CountNewRanking(winner, g.Team1.Capiten.Points, g.Team2.Capiten.Points, w);
                rnewP22 = CountNewRanking(winner, g.Team1.TeamMember.Points, g.Team2.TeamMember.Points, w);

                SetTitle(rnewP11, g.Team1.Capiten);
                SetTitle(rnewP12, g.Team1.TeamMember);
                SetTitle(rnewP21, g.Team2.Capiten);
                SetTitle(rnewP22, g.Team2.TeamMember);

                if (w == 0.5)
                {
                    teamId = g.Team2.Id.ToString(); //u sustini je nevazno koji tim jer jer je remi, samo je poruka bitna. Ali ipak postavljeno
                    g.Team1.Capiten.Draws++;
                    g.Team1.TeamMember.Draws++;
                    g.Team2.Capiten.Draws++;
                    g.Team2.TeamMember.Draws++;
                    g.StatusT1 = GTStatus.Draw;
                    g.StatusT2 = GTStatus.Draw;
                }
                else
                {
                    if (winner)
                    {
                        teamId = g.Team2.Id.ToString();
                        g.Team2.Capiten.Wins++;
                        g.Team2.TeamMember.Wins++;
                        g.Team1.Capiten.Losses++;
                        g.Team1.TeamMember.Losses++;
                        g.StatusT2 = GTStatus.Winner;
                        g.StatusT1 = GTStatus.Losser;
                        g.Team2.Capiten.Tokens += g.Tokens;
                        g.Team2.TeamMember.Tokens += g.Tokens;
                        g.Team1.Capiten.Tokens -= g.Tokens;
                        g.Team1.TeamMember.Tokens -= g.Tokens;
                    }
                    else
                    {
                        teamId = g.Team1.Id.ToString();
                        g.Team2.Capiten.Losses++;
                        g.Team2.TeamMember.Losses++;
                        g.Team1.Capiten.Wins++;
                        g.Team1.TeamMember.Wins++;
                        g.StatusT1 = GTStatus.Winner;
                        g.StatusT2 = GTStatus.Losser;
                        g.Team1.Capiten.Tokens += g.Tokens;
                        g.Team1.TeamMember.Tokens += g.Tokens;
                        g.Team2.Capiten.Tokens -= g.Tokens;
                        g.Team2.TeamMember.Tokens -= g.Tokens;
                    }
                }
            }
            g.Team1.Points = rnewT1;
            g.Team2.Points = rnewT2;
            g.Team1.Capiten.Points = rnewP11;
            g.Team1.TeamMember.Points = rnewP12;
            g.Team2.Capiten.Points = rnewP21;
            g.Team2.TeamMember.Points = rnewP22;

            dbContext.SaveChanges();
            return teamId;
        }

        //rold je stari rating
        //k je koeficijent, racuna se sa k=20
        //ropp je protivnicki rating
        //w je 1.0 ili 0.5; //1.0 za pobedu/poraz a 0.5 za remi; ako je poraz onda se to sto se dobije na desnoj strani oduzima od rold
        //double w = 1.0; //kod nas je uvek pobeda i poraz u atomcu, nema remi (w=1.0 uvek) (ipak ima!)
        //formula je rnew = rold +ili- k/2 * (w+(1.0/2.0)*(abs(rold-ropp))/200)
        private int CountNewRanking(bool win, int rold, int ropp, double w)
        {
            double k = 20;
            double diffInRating = (k / 2) * (1.0 + w * (Math.Abs(rold - ropp) / 200));
            if (w == 0.5)
            {
                if (rold > ropp)
                    return (rold - (int)Math.Round(diffInRating));
                else if (rold < ropp)
                    return (rold + (int)Math.Round(diffInRating));
                else
                    return rold;
            }
            if (win)
                return (rold + (int)Math.Round(diffInRating));
            else
                return (rold - (int)Math.Round(diffInRating));
        }

        private void SetTitle(int ranking, ApplicationUser au)
        {
            if (ranking >= 2600)
                au.Title = Title.GrandMaster;
            else if (ranking >= 2400)
                au.Title = Title.SeniorMaster;
            else if (ranking >= 2200)
                au.Title = Title.Master;
            else if (ranking >= 2000)
                au.Title = Title.Expert;
            else if (ranking >= 1800)
                au.Title = Title.ClassA;
            else if (ranking >= 1600)
                au.Title = Title.ClassB;
            else if (ranking >= 1400)
                au.Title = Title.ClassC;
            else if (ranking >= 1200)
                au.Title = Title.ClassD;
            else if (ranking >= 1000)
                au.Title = Title.Amateur;
            else if (ranking >= 0)
                au.Title = Title.Novice;
        }

    }
}