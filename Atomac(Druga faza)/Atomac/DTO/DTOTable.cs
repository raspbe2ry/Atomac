using Atomac.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    public class DTOTable
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public string Style { get; set; }
        public bool Activity { get; set; }
        public string OwnerId { get; set; }
        public DTOAppUser Owner { get; set; }

        public DTOTable GetById(int id)
        {
            var table = dbContext.ATable.Find(id);
            return Mapper.Map<DTOTable>(table);
        }
    }
}