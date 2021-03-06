﻿using Atomac.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class AFigure: Artifact
    {
        public override DTOArtifact GetDTOArtifact()
        {
            DTOFigure f = new DTOFigure();
            return f.GetById(this.Id);
        }
    }
}