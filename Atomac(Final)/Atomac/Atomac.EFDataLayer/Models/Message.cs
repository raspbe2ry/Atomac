using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.EFDataLayer.Models
{
    public class Message
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength (256)]
        [Required]
        public string Text { get; set; }
        public bool LinkTag { get; set; }

        [DataType (DataType.DateTime)]
        public DateTime Time { get; set; }

        public string SenderId { get; set; }
        [ForeignKey ("SenderId")]
        public virtual ApplicationUser Sender { get; set; }
    }
}