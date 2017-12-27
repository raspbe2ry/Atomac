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
    public class AFiguresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AFigures
        public ActionResult Index()
        {
            List<DTOFigure> lista = new List<DTOFigure>();
            List<AFigure> artifacts = db.AFigures.ToList<AFigure>();
            foreach (AFigure a in artifacts)
            {
                DTOFigure figure = new DTOFigure();
                lista.Add(figure.GetById(a.Id));
            }
            return View(lista);
        }

        // GET: AFigures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTOFigure aFigure = new DTOFigure();
            aFigure=aFigure.GetById((int)id);
            if (aFigure == null)
            {
                return HttpNotFound();
            }
            return View(aFigure);
        }

        // GET: AFigures/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: AFigures/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Style,Activity,OwnerId")] AFigure aFigure)
        {
            if (ModelState.IsValid)
            {
                db.AFigures.Add(aFigure);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aFigure.OwnerId);
            return View(aFigure);
        }

        // GET: AFigures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFigure aFigure = db.AFigures.Find(id);
            if (aFigure == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aFigure.OwnerId);
            return View(aFigure);
        }

        // POST: AFigures/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Style,Activity,OwnerId")] AFigure aFigure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aFigure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aFigure.OwnerId);
            return View(aFigure);
        }

        // GET: AFigures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AFigure aFigure = db.AFigures.Find(id);
            if (aFigure == null)
            {
                return HttpNotFound();
            }
            return View(aFigure);
        }

        // POST: AFigures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AFigure aFigure = db.AFigures.Find(id);
            db.AFigures.Remove(aFigure);
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
