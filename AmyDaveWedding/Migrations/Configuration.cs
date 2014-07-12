namespace AmyDaveWedding.Migrations
{
    using AmyDaveWedding.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<AmyDaveWedding.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AmyDaveWedding.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            
            //if( !context.Users.Any(u => u.UserName == "eisnel") )
            //{
            //    var roleStore = new RoleStore<IdentityRole>(context);
            //    var roleManager = new RoleManager<IdentityRole>(roleStore);
            //    var userStore = new UserStore<ApplicationUser>(context);
            //    var userManager = new UserManager<ApplicationUser>(userStore);

            //    var user = new ApplicationUser { UserName = "davep" };

            //    userManager.Create(user, "test123");
            //    roleManager.Create(new IdentityRole { Name = "admin" });
            //    userManager.AddToRole(user.Id, "admin");
            //}

            if( !context.Invitees.Any( i => i.Name == "Dave Poyas"))
            {
                var invitee = new Invitee
                {
                    Name = "Dave Poyas",
                    ZipCode = "55369",
                    IsKnown = true,
                    InvitedToRehearsal = true,
                    Group = "betrothed"
                };
                context.Invitees.Add(invitee);
                context.SaveChanges();
            }
            if (!context.Invitees.Any(i => i.Name == "Amy Keast"))
            {
                var invitee = new Invitee
                {
                    Name = "Amy Keast",
                    ZipCode = "55369",
                    IsKnown = true,
                    InvitedToRehearsal = true,
                    Group = "betrothed"
                };
                context.Invitees.Add(invitee);
                context.SaveChanges();
            }
        }
    }
}
