using Atomac.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class ATable: Artifact
    {
        public override DTOArtifact GetDTOArtifact()
        {
            DTOTable f = new DTOTable();
            return f.GetById(this.Id);
        }
    }
}