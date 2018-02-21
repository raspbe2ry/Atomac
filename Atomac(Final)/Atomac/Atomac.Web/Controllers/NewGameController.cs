using Atomac.EFDataLayer.DTO;
using Atomac.EFDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atomac.Controllers
{
    public class NewGameController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index(string id)
        {
            int gameId = int.Parse(id);
            DTOGame dtoGame = new DTOGame();
            dtoGame = dtoGame.GetById(gameId);
            ViewBag.FirstTeam = dtoGame.Team1Id;
            ViewBag.SecondTeam = dtoGame.Team2Id;
            string userEmail = User.Identity.Name;
            ViewBag.NickName = db.Users.Where(x => x.Email.Equals(userEmail)).First().NickName;

            if (dtoGame.Team1.Capiten.Email == User.Identity.Name || dtoGame.Team1.TeamMember.Email == User.Identity.Name)
            {
                ViewBag.CapId = dtoGame.Team1.Capiten.Email;
                ViewBag.MyTeamId = dtoGame.Team1Id;
            }
            else
            {
                ViewBag.CapId = dtoGame.Team2.Capiten.Email;
                ViewBag.MyTeamId = dtoGame.Team2Id;
            }

            return View(dtoGame);   
        }

    }
}