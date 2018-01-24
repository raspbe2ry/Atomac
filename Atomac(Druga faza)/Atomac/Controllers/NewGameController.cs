using Atomac.DTO;
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
        public ActionResult Index(string id)
        {
            int gameId = Int32.Parse(id);
            DTOGame dtoGame = new DTOGame();
            dtoGame = dtoGame.GetById(gameId);
            return View(dtoGame);   
        }
    }
}