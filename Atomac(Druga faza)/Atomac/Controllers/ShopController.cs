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
using Atomac.ViewModels;
using System.Reflection;

namespace Atomac.Controllers
{
    public class ShopController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Shop
        public ActionResult Index()
        {
            ApplicationUser player = db.Users.Where(x => x.Email == User.Identity.Name).First();
            ViewBag.UserId = player.Id;

            Type parentType = typeof(Artifact);
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.BaseType == parentType);
            List<Artifact> art = db.Artifacts.ToList();
            List<ListType> lista = new List<ListType>(); //za povratak
            foreach(Type type in subclasses)
            {
                ListType lt = new ListType();
                lt.name = type.Name;
                lt.name = type.Name.Substring(1);
                foreach(Artifact ar in art)
                {
                    if (ar.GetType().Name.Contains(type.Name))
                    {
                        //lis.Add(ar);
                        ShopModel sm = new ShopModel();
                        sm.Id = ar.Id;
                        sm.Prize = ar.Prize;
                        sm.Style = ar.Style;
                        bool found = false;
                        foreach(Stuff s in player.Stuffs)
                        {
                            if(s.ArtifactId==ar.Id)
                            {
                                if (s.Activity == true)
                                    sm.Status = AStatus.Active;
                                else
                                    sm.Status = AStatus.NotActive;
                                found = true;
                            }
                        }
                        if (found == false)
                            sm.Status = AStatus.NeedToBuy;
                        lt.shopModels.Add(sm);
                    }
                }
                lista.Add(lt);
            }
            return View(lista);
        }

        // GET: Shop/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artifact artifact = db.Artifacts.Find(id);
            if (artifact == null)
            {
                return HttpNotFound();
            }
            return View(artifact);
        }

        // GET: Shop/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Shop/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Style")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                db.Artifacts.Add(artifact);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(artifact);
        }

        // GET: Shop/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artifact artifact = db.Artifacts.Find(id);
            if (artifact == null)
            {
                return HttpNotFound();
            }
            return View(artifact);
        }

        // POST: Shop/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Style")] Artifact artifact)
        {
            if (ModelState.IsValid)
            {
                db.Entry(artifact).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(artifact);
        }

        // GET: Shop/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Artifact artifact = db.Artifacts.Find(id);
            if (artifact == null)
            {
                return HttpNotFound();
            }
            return View(artifact);
        }

        // POST: Shop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Artifact artifact = db.Artifacts.Find(id);
            db.Artifacts.Remove(artifact);
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

        [HttpPost]
        public ActionResult BuyArtifact()
        {
            int artifactId = Int32.Parse(Request.Form["artifactId"]);
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Artifact art = db.Artifacts.Find(artifactId);
                ApplicationUser user = db.Users.Find(userId);
                Stuff s = new Stuff();
                s.Activity = false;
                s.Artifact = art;
                s.Owner = user;
                user.Stuffs.Add(s);
                art.Stuffs.Add(s);
                db.SaveChanges();
            }
            return Json(artifactId, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult ActivateArtifact()
        {
            ReturnAfterActivation raa = new ReturnAfterActivation(); //objekat u kome nalaze o aktiviranom i deaktiviranom artefaktu
            int artifactId = Int32.Parse(Request.Form["artifactId"]);
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Stuff st = db.Stuffs.Where(x => x.ArtifactId == artifactId && x.OwnerId == userId).ToList().First();
                string typeName = st.Artifact.GetType().Name;
                ApplicationUser user = db.Users.Find(userId);
                foreach(Stuff s in user.Stuffs)
                {
                    if (s.Artifact.GetType().Name == typeName && s.Activity == true)
                    {
                        s.Activity = false;
                        raa.deactivated = s.ArtifactId.ToString();
                        break;
                    }
                }
                st.Activity = true;
                raa.activated = st.ArtifactId.ToString();
                db.SaveChanges();
            }
            return Json(raa, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeactivateArtifact()
        {
            ReturnAfterActivation raa = new ReturnAfterActivation(); //objekat u kome nalaze o aktiviranom i deaktiviranom artefaktu
            int artifactId = Int32.Parse(Request.Form["artifactId"]);
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Stuff st = db.Stuffs.Where(x => x.ArtifactId == artifactId && x.OwnerId == userId).ToList().First();
                string typeName = st.Artifact.GetType().Name;
                ApplicationUser user = db.Users.Find(userId);
                foreach (Stuff s in user.Stuffs)
                {
                    if (s.Artifact.GetType().Name == typeName && s.Artifact.Style=="default")
                    {
                        s.Activity = true;
                        raa.activated = s.ArtifactId.ToString();
                        break;
                    }
                }
                st.Activity = false;
                raa.deactivated = st.ArtifactId.ToString();
                db.SaveChanges();

                Artifact artif = new ATable();
                artif.Prize = 52;
                artif.Style = "dddd";
                db.Artifacts.Add(artif);
                db.SaveChanges();
            }
            return Json(raa, JsonRequestBehavior.AllowGet);
        }
    }
}
