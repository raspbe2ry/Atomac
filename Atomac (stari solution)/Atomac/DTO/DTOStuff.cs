using Atomac.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOStuff
    {
        private ApplicationDbContext dbContext = new ApplicationDbContext();

        public int Id { get; set; }
        public bool Activity { get; set; }
        public string ArtifactId { get; set; }
        public string OwnerId { get; set; }
        public DTOArtifactMini Artifact { get; set; }

        public DTOStuff GetById(int id)
        {
            var figure = dbContext.AFigures.Find(id);
            return Mapper.Map<DTOStuff>(figure);
        }
    }
}