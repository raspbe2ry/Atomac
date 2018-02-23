using Atomac.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Atomac.Models
{
    public class Artifact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Style { get; set; }

        [Required]
        public int Prize { get; set; }

        public virtual ICollection<Stuff> Stuffs { get; set; }

        public Artifact()
        {
            Stuffs = new List<Stuff>();
        }

        public virtual DTOArtifact GetDTOArtifact()
        {
            return null;
        }
    }
}