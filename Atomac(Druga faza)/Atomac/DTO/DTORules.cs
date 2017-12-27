using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Atomac.Models;
using AutoMapper;

namespace Atomac.DTO
{
    public class DTORules
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public bool DroppedCheck { get; set; }  //ako je promenljiva setovana na true onda se ovo pravilo odnosi na zadatu partiju
        public bool DroppedCheckMate { get; set; }
        public bool DroppedPawnOnFirstLine { get; set; }
        public bool DroppedPawnOnLastLine { get; set; }
        public bool DroppedFigureOnLastLine { get; set; }

        public DTORules GetById(int id)
        {
            var rules = dbContext.Ruless.Find(id);
            return Mapper.Map<DTORules>(rules);
        }
    }

}