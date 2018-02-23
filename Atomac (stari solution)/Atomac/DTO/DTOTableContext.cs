using Atomac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOTableContext
    {
        public string table;
        public string figure;

        public DTOTableContext(string userMail)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {                
                List<Stuff> stuffs = db.Users.Where(x=>x.UserName==userMail).ToList().First().Stuffs.Where(y=>y.Activity==true).ToList();
                foreach(Stuff s in stuffs)
                {
                    if (s.Artifact is ATable)
                        table = s.Artifact.Style;
                    else if (s.Artifact is AFigure)
                        figure = s.Artifact.Style;
                }
            }
        }
    }
}