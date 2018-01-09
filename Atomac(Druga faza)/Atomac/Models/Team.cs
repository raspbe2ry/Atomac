using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength (64)]
        [Required]
        public string Name { get; set; }
        public TStatus Status { get; set; }
        public int Points { get; set; }

        public string CapitenId { get; set; }
        [ForeignKey("CapitenId")]
        public virtual ApplicationUser Capiten { get; set; }

        public string TeamMemberId { get; set; }
        [ForeignKey("TeamMemberId")]
        public virtual ApplicationUser TeamMember { get; set; }

        [InverseProperty ("Team1")]
        public virtual ICollection<Game> GamesAsFirst { get; set; }

        [InverseProperty("Team2")]
        public virtual ICollection<Game> GamesAsSecond { get; set; }

        public Team()
        {
            GamesAsFirst = new List<Game>();
            GamesAsSecond = new List<Game>();
        }
    }
    
    public enum TStatus
    {
        Offline=1,//bar jedan od igraca igraca je offline
        Online=2,//oba igraca su aktivna
        Busy=3,//makar 1 igrac je InGame, a drugi je aktivan
        Active=4//oba igraca su aktivna + oba igraca su selektovala da igraju u njemu
    }
}