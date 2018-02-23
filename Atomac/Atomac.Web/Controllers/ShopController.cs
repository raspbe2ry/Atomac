using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Atomac.Web.ViewModels;
using System.Reflection;
using System.Web.Script.Serialization;
using Atomac.EFDataLayer.Models;
using Atomac.EFDataLayer;
using Atomac.EFDataLayer.DTO;

namespace Atomac.Controllers
{
    public class ShopController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            ApplicationUser player = db.Users.Where(x => x.Email == User.Identity.Name).First();
            ViewBag.UserId = player.Id;
            ViewBag.UserTokens = player.Tokens;

            Type parentType = typeof(Artifact);
            Assembly assembly = Assembly.GetAssembly(parentType);
            Type[] types = assembly.GetTypes();
            IEnumerable<Type> subclasses = types.Where(t => t.BaseType == parentType);
            List<Artifact> art = db.Artifacts.ToList();
            List<ListType> lista = new List<ListType>(); //za povratak
            foreach(Type type in subclasses)
            {
                ListType lt = new ListType();
                lt.name = type.Name;
                lt.name = type.Name.Substring(1);
                foreach (Artifact ar in art)
                {
                    if (ar.GetType().Name.Contains(type.Name))
                    {
                        //lis.Add(ar);
                        ShopModel sm = new ShopModel();
                        sm.Id = ar.Id;
                        sm.Prize = ar.Prize;
                        sm.Style = ar.Style;
                        bool found = false;
                        foreach (Stuff s in player.Stuffs)
                        {
                            if (s.ArtifactId == ar.Id)
                            {
                                sm.Status = (s.Activity) ? AStatus.Active : AStatus.NotActive;
                                found = true;
                            }
                        }
                        if (!found)
                            sm.Status = AStatus.NeedToBuy;
                        lt.shopModels.Add(sm);
                    }
                }
                lista.Add(lt);
            }

            DTOTableContext dt = new DTOTableContext(User.Identity.Name);
            var json=new JavaScriptSerializer().Serialize(dt);
            ViewBag.TableContext = json;

            return View(lista);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult BuyArtifact()
        {
            BoughtArtifact bArtifact = new BoughtArtifact();
            int artifactId = int.Parse(Request.Form["artifactId"]);
            bArtifact.artifactId = artifactId;
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Artifact art = db.Artifacts.Find(artifactId);
                bArtifact.prize = art.Prize;
                ApplicationUser user = db.Users.Find(userId);
                user.Tokens -= art.Prize;
                Stuff s = new Stuff();
                s.Activity = false;
                s.Artifact = art;
                s.Owner = user;
                user.Stuffs.Add(s);
                art.Stuffs.Add(s);
                db.SaveChanges();
            }

            return Json(bArtifact, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult ActivateArtifact()
        {
            ReturnAfterActivation raa = new ReturnAfterActivation(); //objekat u kome nalaze o aktiviranom i deaktiviranom artefaktu
            int artifactId = int.Parse(Request.Form["artifactId"]);
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Stuff st = db.Stuffs.Where(x => x.ArtifactId == artifactId && x.OwnerId == userId).ToList().First();
                string typeName = st.Artifact.GetType().Name;
                ApplicationUser user = db.Users.Find(userId);
                foreach (Stuff s in user.Stuffs)
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
            int artifactId = int.Parse(Request.Form["artifactId"]);
            string userId = Request.Form["userId"];
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Stuff st = db.Stuffs.Where(x => x.ArtifactId == artifactId && x.OwnerId == userId).ToList().First();
                string typeName = st.Artifact.GetType().Name;
                ApplicationUser user = db.Users.Find(userId);
                foreach (Stuff s in user.Stuffs)
                {
                    if (s.Artifact.GetType().Name == typeName && s.Artifact.Style == "default")
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
