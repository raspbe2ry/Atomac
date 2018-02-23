using Atomac.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOFigure: DTOArtifact
    {
        public override DTOArtifact GetById(int id)
        {
            var figure = dbContext.AFigures.Find(id);
            return Mapper.Map<DTOFigure>(figure);
        }
    }
}