using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.Models
{
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Board Board { get; set; }
        public string Color { get; set; } //koja boja je odigrala potez
        public string Captured { get; set; } //ako je figura uzeta, koja je
        public string State { get; set; }
        public IList<string> White { get; set; }  //koje figure beli ima u rezervi
        public IList<string> Black { get; set; }  //koje figure crni ima u rezervi

        public int GameId { get; set; }
        [ForeignKey ("GameId")]
        public virtual Game Game { get; set; }

        public Move()
        {
            White = new List<string>();
            Black = new List<string>();
        }
    }

}