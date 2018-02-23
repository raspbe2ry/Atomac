using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atomac.Models;
using AutoMapper;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOTeam
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }
        public string Name { get; set; }
        public TStatus Status { get; set; }
        public int Points { get; set; }
        public string CapitenId { get; set; }
        public virtual DTOAppUserMini Capiten { get; set; }
        public string TeamMemberId { get; set; }
        public DTOAppUserMini TeamMember { get; set; }

        public DTOTeam GetById(int id)
        {
            var team = db.Teams.Find(id);
            return Mapper.Map<DTOTeam>(team);
        }
    }
}