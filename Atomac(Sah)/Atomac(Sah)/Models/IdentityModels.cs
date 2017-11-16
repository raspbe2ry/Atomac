using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Atomac_Sah_.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(64)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(64)]
        public string LastName { get; set; }
        [Required]
        [StringLength(64)]
        public string NickName { get; set; }
        public int Points { get; set; }
        public int Tokens { get; set; }
        public string Picture { get; set; }  //putanja do slike 
        public Title Title { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }

        //NavigationProperty
        public virtual ICollection<AFigure> FigureStyles { get; set; }
        public virtual ICollection<ATable> TableStyles { get; set; }
        public virtual ICollection<Message> Messages { get; set; }

        [InverseProperty("TeamMember")]
        public virtual ICollection<Team> Teams { get; set; }

        [InverseProperty("Capiten ")]
        public virtual ICollection<Team> AdminedTeams { get; set; }  //mozda ovo ne treba, mozda ce DTO objekat sadrzati adminovane ekipe

        public ApplicationUser()
        {
            FigureStyles = new List<AFigure>();
            TableStyles = new List<ATable>();
            Messages = new List<Message>();
            Teams = new List<Team>();
            AdminedTeams = new List<Team>(); //ako se obrise gore treba da se obrise i ovde
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

    }

    public enum Title
    {
        Novice=0,
        Amateur =1000, 
        ClassD =1200,
        ClassC= 1400,   
        ClassB =1600,   
        ClassA =1800,  
        Expert=2000,  
        Master=2200,  
        SeniorMaster=2400,  
        GrandMaster=2600,  
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("AtomacBaza", throwIfV1Schema: false)
        {

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public virtual DbSet<AFigure> AFigures { get; set; }
        public virtual DbSet<ATable> ATables { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Move> Moves { get; set; }
        public virtual DbSet<Rules> Ruless { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
    }
}