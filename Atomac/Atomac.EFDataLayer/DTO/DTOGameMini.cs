using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.DTO
{
    public class DTOGameMini
    {
        public string Id { get; set; }
        public string Tokens { get; set; }
        public string Duration { get; set; }
        public string TeamId { get; set; }
        public string DroppedCheck { get; set; }
        public string DroppedCheckMate { get; set; }
        public string DroppedPawnOnFirstLine { get; set; }
        public string DroppedPawnOnLastLine { get; set; }
        public string DroppedFigureOnLastLine { get; set; }
    }
}