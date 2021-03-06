﻿using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;

namespace AmyDaveWedding.Models
{
    /*
     * To update using Code First Migrations:
     * Open the Package Manager Console and run: update-database
     * If you get the error: "Automatic migration was not applied because it would result in data loss"
     * then you'll need to destroy data using: update-database -force
     * http://msdn.microsoft.com/en-us/data/jj591621.aspx
     */

    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() : base()
        {
            CreationDate = DateTime.Now;
        }

        public Invitee Invitee { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime CreationDate { get; set; }

        //public string Code { get; set; }

        public DateTime? RsvpDate { get; set; }

        public bool? Attending { get; set; }

        //public int? AdditionalAdultCount { get; set; }

        //public int? ChildCount { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Invitee> Invitees { get; set; }

        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
    }
}