using Atomac.EFDataLayer.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.DTO
{
    public class DTOAppUserMini
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string LastName { get; set; }
        public string NickName { get; set; }
        public int Points { get; set; }
        public int Tokens { get; set; }
        public string Picture { get; set; }
        public Title Title { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public PStatus Status { get; set; }

        public DTOAppUserMini GetById(string id)
        {
            var user = db.Users.Find(id);
            return Mapper.Map<DTOAppUserMini>(user);
        }
    }
}