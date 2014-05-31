using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AmyDaveWedding.Models
{
    public class WeddingContext : DbContext
    {
        public WeddingContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Invitee> Invitees { get; set; }
    }
}