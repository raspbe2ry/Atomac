using Atomac.EFDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.DTO
{
    [Serializable]
    public abstract class DTOArtifact
    {
        protected ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public int Prize { get; set; }
        public string Style { get; set; }
        public ICollection<DTOStuff> Stuffs { get; set; }

        public abstract DTOArtifact GetById(int id);
    }
}