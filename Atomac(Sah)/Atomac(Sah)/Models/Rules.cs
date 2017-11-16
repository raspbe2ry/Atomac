using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac_Sah_.Models
{
    public class Rules
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public bool DroppedCheck { get; set; }  //ako je promenljiva setovana na true onda se ovo pravilo odnosi na zadatu partiju
        public bool DroppedCheckMate { get; set; }
        public bool DroppedPawnOnFirstLine { get; set; }
        public bool DroppedPawnOnLastLine { get; set; }
        public bool DroppedFigureOnLastLine { get; set; }
    }
}