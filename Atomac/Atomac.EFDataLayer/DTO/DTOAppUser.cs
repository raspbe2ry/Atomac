using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Atomac.EFDataLayer.Models;

namespace Atomac.EFDataLayer.DTO
{
    [Serializable]
    public class DTOAppUser
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int Points { get; set; }
        public int Tokens { get; set; }
        public string Picture { get; set; }  //putanja do slike 
        public Title Title { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public PStatus Status { get; set; }
        public ICollection<DTOTeam> Teams { get; set; }
        public ICollection<DTOTeam> AdminedTeams { get; set; }
        public ICollection<DTOStuff> Stuffs { get; set; }

        public DTOAppUser GetById(string id)
        {
            var user = db.Users.Find(id);
            return Mapper.Map<DTOAppUser>(user);
        }
    }
}