using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac_Sah_.Models
{
    public class Move
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public Board Board { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string White { get; set; }  //koje figure beli ima u rezervi
        [Required]
        public string Black { get; set; }  //koje figure crni ima u rezervi

        public int GameId { get; set; }
        [ForeignKey ("GameId")]
        public virtual Game Game { get; set; }
    }

    public enum Board
    {
        T1 =1,
        T2=2
    }
}