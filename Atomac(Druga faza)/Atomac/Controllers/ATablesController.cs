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
    public class ATablesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ATables
        public ActionResult Index()
        {
            List<DTOTable> lista = new List<DTOTable>();
            List<ATable> artifacts = db.ATable.ToList<ATable>();
            foreach(ATable a in artifacts)
            {
                DTOTable table = new DTOTable();
                lista.Add(table.GetById(a.Id));
            }
            return View(lista);
        }

        // GET: ATables/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DTOTable aTable=new DTOTable();
            aTable=aTable.GetById((int)id);
            if (aTable == null)
            {
                return HttpNotFound();
            }
            return View(aTable);
        }

        // GET: ATables/Create
        public ActionResult Create()
        {
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: ATables/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Style,Activity,OwnerId")] ATable aTable)
        {
            if (ModelState.IsValid)
            {
                db.ATable.Add(aTable);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aTable.OwnerId);
            return View(aTable);
        }

        // GET: ATables/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATable aTable = db.ATable.Find(id);
            if (aTable == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aTable.OwnerId);
            return View(aTable);
        }

        // POST: ATables/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Style,Activity,OwnerId")] ATable aTable)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aTable).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", aTable.OwnerId);
            return View(aTable);
        }

        // GET: ATables/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ATable aTable = db.ATable.Find(id);
            if (aTable == null)
            {
                return HttpNotFound();
            }
            return View(aTable);
        }

        // POST: ATables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ATable aTable = db.ATable.Find(id);
            db.ATable.Remove(aTable);
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
