using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class Game
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int Points { get; set; }

        [Required]
        public int Tokens { get; set; }

        [Required]
        public int Duration { get; set; }

        public GStatus Status { get; set; }

        public int Team1Id { get; set; }
        [ForeignKey ("Team1Id")]
        public virtual Team Team1 { get; set; }

        public int Team2Id { get; set; }
        [ForeignKey("Team2Id")]
        public virtual Team Team2 { get; set; }

        public int RulesId { get; set; }
        [ForeignKey("RulesId")]
        public virtual Rules Rules { get; set; }

        public virtual ICollection<Move> Moves { get; set; }

        public Game()
        {
            Moves = new List<Move>();
        }
    }

    public enum GStatus
    {
        Created = 1, 
        Playing = 2
    }
}