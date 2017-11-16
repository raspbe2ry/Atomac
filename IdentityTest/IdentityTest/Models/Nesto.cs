using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace IdentityTest.Models
{
    public class Nesto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int KlasaId { get; set; }
        [ForeignKey("KlasaId")]
        public virtual Klasa Klasa { get; set; }
    }
}