using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atomac.Models;
using AutoMapper;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOMove
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public int Id { get; set; }
        public Board Board { get; set; }
        public string State { get; set; }
        public string White { get; set; }  //koje figure beli ima u rezervi
        public string Black { get; set; }  //koje figure crni ima u rezervi
        public int GameId { get; set; }

        public DTOMove GetById(int id)
        {
            var move = db.Moves.Find(id);
            return Mapper.Map<DTOMove>(move);
        }
    }
}