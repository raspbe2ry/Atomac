using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Atomac.Models;
using Atomac.DTO;

namespace Atomac.Controllers
{
    public class GamesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Games
        public ActionResult Index()
        {
            List<DTOGame> lista = new List<DTOGame>();
            List<Game> games = db.Games.ToList<Game>();
            foreach (Game a in games)
            {
                DTOGame ga = new DTOGame();
                lista.Add(ga.GetById(a.Id));
            }
            return View(lista);
        }

        // GET: Games/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTOGame game = new DTOGame();
            game = game.GetById((int)id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // GET: Games/Create
        public ActionResult Create()
        {
            ViewBag.RulesId = new SelectList(db.Ruless, "Id", "Id");
            ViewBag.Team1Id = new SelectList(db.Teams, "Id", "Name");
            ViewBag.Team2Id = new SelectList(db.Teams, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Date,Points,Tokens,Duration,Status,Team1Id,Team2Id,RulesId")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.RulesId = new SelectList(db.Ruless, "Id", "Id", game.RulesId);
            ViewBag.Team1Id = new SelectList(db.Teams, "Id", "Name", game.Team1Id);
            ViewBag.Team2Id = new SelectList(db.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // GET: Games/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            ViewBag.RulesId = new SelectList(db.Ruless, "Id", "Id", game.RulesId);
            ViewBag.Team1Id = new SelectList(db.Teams, "Id", "Name", game.Team1Id);
            ViewBag.Team2Id = new SelectList(db.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Date,Points,Tokens,Duration,Status,Team1Id,Team2Id,RulesId")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RulesId = new SelectList(db.Ruless, "Id", "Id", game.RulesId);
            ViewBag.Team1Id = new SelectList(db.Teams, "Id", "Name", game.Team1Id);
            ViewBag.Team2Id = new SelectList(db.Teams, "Id", "Name", game.Team2Id);
            return View(game);
        }

        // GET: Games/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = db.Games.Find(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Game game = db.Games.Find(id);
            db.Games.Remove(game);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
