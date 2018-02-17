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
using System.Web.Script.Serialization;

namespace Atomac.Controllers
{
    public class ProfileController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Profile/Details/5
        public ActionResult Details()
        {
            ApplicationUser applicationUser = db.Users.Where(x=>x.Email==User.Identity.Name).ToList().First();
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            DTOAppUser user = new DTOAppUser();
            user=user.GetById(applicationUser.Id);
            return View(user);
        }
        
        //Get Profile/MyTableContext
        public string MyTableContext()
        {
            string userEmail = HttpContext.Request.Params.Get("userEmail");
            DTOTableContext dto = new DTOTableContext(userEmail);
            var json = new JavaScriptSerializer().Serialize(dto);
            return json;
        }
    }
}
