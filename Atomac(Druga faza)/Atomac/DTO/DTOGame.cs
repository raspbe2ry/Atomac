using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atomac.Models;
using AutoMapper;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOGame
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Tokens { get; set; }
        public int Duration { get; set; }
        public GStatus Status { get; set; }
        public int Team1Id { get; set; }
        public DTOTeam Team1 { get; set; }
        public int? Team2Id { get; set; }
        public DTOTeam Team2 { get; set; }
        public int? RulesId { get; set; }
        public DTORules Rules { get; set; }
        public ICollection<DTOMove> Moves { get; set; }

        public DTOGame GetById(int id)
        {
            var game = db.Games.Find(id);
            return Mapper.Map<DTOGame>(game);
        }
    }
}