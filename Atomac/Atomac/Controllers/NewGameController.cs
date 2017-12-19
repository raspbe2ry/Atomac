using Atomac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atomac.Controllers
{
    public class NewGameController : Controller
    {
        public ActionResult Index()
        {

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                ApplicationUser c1 = db.Users.FirstOrDefault(x => x.UserName == "dima@gmal.com");
                ApplicationUser c2 = db.Users.FirstOrDefault(x => x.UserName == "nmalinovic@gmail.com");
                ApplicationUser c3 = db.Users.FirstOrDefault(x => x.UserName == "nlugic@gmail.com");
                ApplicationUser c4 = db.Users.FirstOrDefault(x => x.UserName == "pantela@gmail.com");

                Team t1 = new Team
                {
                    Name = "Izazivaci",
                    Status = TStatus.Busy,
                    Points = 2500,
                    Capiten=c1,
                    TeamMember=c2
                };
                
                Team t2 = new Team
                {
                    Name = "Izazvani",
                    Status = TStatus.Busy,
                    Points = 2500,
                    Capiten = c3,
                    TeamMember = c4
                };


                db.Teams.Add(t1);
                db.Teams.Add(t2);
                //c1.Teams.Add(t1);
                //c2.Teams.Add(t1);
                //c3.Teams.Add(t2);
                //c4.Teams.Add(t2);

                Rules r = new Rules
                {
                    DroppedCheck = true,
                    DroppedCheckMate = false,
                    DroppedFigureOnLastLine = false,
                    DroppedPawnOnFirstLine = true,
                    DroppedPawnOnLastLine = true
                };

                db.Ruless.Add(r);
                db.SaveChanges();

                Game g = new Game
                {
                    Date = DateTime.Now,
                    Points = 100,
                    Tokens = 150,
                    Duration = 15,
                    Status = GStatus.Created,
                    Team1 = t1,
                    Team2 = t2,
                    Rules = r
                };

                db.Games.Add(g);
                db.SaveChanges();

                return View(g);
            }
            
        }
    }
}