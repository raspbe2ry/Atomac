using Atomac.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOFigure
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public string Style { get; set; }
        public bool Activity { get; set; }
        public string OwnerId { get; set; }

        public DTOFigure GetById(int id)
        {
            var figure = dbContext.AFigures.Find(id);
            return Mapper.Map<DTOFigure>(figure);
        }

    }
}