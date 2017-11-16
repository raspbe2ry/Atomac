using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class AFigure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Style { get; set; }

        [Required]
        public bool Activity {  get; set;}

        public string OwnerId { get; set; }
        [ForeignKey ("OwnerId")]
        public virtual ApplicationUser Owner {get; set;}
    }
}