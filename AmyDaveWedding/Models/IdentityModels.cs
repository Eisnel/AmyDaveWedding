using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace AmyDaveWedding.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            CreationDate = DateTime.Now;
            Attending = false;
        }

        public string Email { get; set; }

        public string Name { get; set; }

        //public string Code { get; set; }

        public bool Attending { get; set; }

        //public int? AdditionalAdultCount { get; set; }

        //public int? ChildCount { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? RsvpDate { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}