using Atomac.EFDataLayer.DTO;
using Atomac.EFDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Atomac.Controllers
{
    public class HomeController : Controller
    {

        private ApplicationDbContext dbContext = new ApplicationDbContext();

        [HttpGet]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("About", "Home");
            else
                return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            string mail = User.Identity.Name;
            string nick = dbContext.Users.Where(b => b.Email == mail).FirstOrDefault().NickName;
            ViewBag.NickName = nick;
            //collection.OrderByDescending(c => c.Key).Take(3).OrderBy(c => c.Key);
            //List<Message> msgs = dbContext.Messages.Skip(Math.Max(0, dbContext.Messages.Count() - 8)).ToList();
            List<Message> msgs = dbContext.Messages.OrderByDescending(c => c.Time).Take(300).OrderBy(c => c.Time).ToList();
            List<DTOMessageMini> msgsMini = new List<DTOMessageMini>();
            foreach(Message m in msgs)
            {
                DTOMessageMini mm = new DTOMessageMini();
                mm.Text = m.Text;
                mm.Time = m.Time.ToString();
                mm.Sender = m.Sender.NickName;
                mm.LinkTag = m.LinkTag;
                msgsMini.Add(mm);
            }
            return View(msgsMini);
        }

        [HttpGet]
        public ActionResult Rankings()
        {
            return View();
        }

    }
}