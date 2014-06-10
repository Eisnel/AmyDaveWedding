using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AmyDaveWedding.Models
{
    /*
     * To update using Code First Migrations:
     * Open the Package Manager Console and run: update-database
     * If you get the error: "Automatic migration was not applied because it would result in data loss"
     * then you'll need to destroy data using: update-database -force
     * http://msdn.microsoft.com/en-us/data/jj591621.aspx
     */
    public class WeddingContext : DbContext
    {
        public WeddingContext()
            : base("DefaultConnection")
        {
        }

        //public DbSet<Invitee> Invitees { get; set; }
    }
}