﻿using System;
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

        public int Points { get; set; }

        public int Tokens { get; set; }

        public int Duration { get; set; }

        public GStatus Status { get; set; }

        public GTStatus StatusT1 { get; set; }

        public GTStatus StatusT2 { get; set; }

        public int Team1Id { get; set; }
        [ForeignKey ("Team1Id")]
        public virtual Team Team1 { get; set; }

        public int? Team2Id { get; set; }
        [ForeignKey("Team2Id")]
        public virtual Team Team2 { get; set; }

        public int? RulesId { get; set; }
        [ForeignKey("RulesId")]
        public virtual Rules Rules { get; set; }

        public virtual ICollection<Move> Moves { get; set; }

        public Game()
        {
            Moves = new List<Move>();
        }
    }

    public enum GStatus  //game status
    {
        Created = 1, 
        Playing = 2
    }

    public enum GTStatus  //game team status
    {
        Prepare=1, //admin selektuje zeljena pravila
        Ready=2, // admin je selektovao pravila
        Winner=3, 
        Losser=4
    }
}