using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac_Sah_.Models
{
    public class Team
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength (64)]
        [Required]
        public string Name { get; set; }
        public bool Activity { get; set; }
        public int Points { get; set; }

        public string CapitenId { get; set; }
        [ForeignKey("CapitenId")]
        public virtual ApplicationUser Capiten { get; set; }

        public string TeamMemberId { get; set; }
        [ForeignKey("TeamMemberId")]
        public virtual ApplicationUser TeamMember { get; set; }

        public virtual ICollection<Game> Games { get; set; }

        public Team()
        {
            Games = new List<Game>();
        }
    }
}