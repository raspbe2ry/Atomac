using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.DTO
{
    [Serializable]
    public class DTOArtifactMini
    {
        public int Id { get; set; }
        public int Prize { get; set; }
        public string Style { get; set; }
        public string Name { get; set; }
    }
}