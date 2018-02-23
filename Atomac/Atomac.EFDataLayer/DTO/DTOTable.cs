using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.DTO
{
    [Serializable]
    public class DTOTable: DTOArtifact
    {
        public override DTOArtifact GetById(int id)
        {
            var table = dbContext.ATable.Find(id);
            return Mapper.Map<DTOTable>(table);
        }
    }
}