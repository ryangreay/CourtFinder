namespace CourtFinder.Migrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CourtFinder.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(CourtFinder.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            //context.Roles.AddOrUpdate(new IdentityRole { Name = "Player" });
            //context.Roles.AddOrUpdate(new IdentityRole { Name = "FacilityManager" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Basketball" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Baseball" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Soccer" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Volleyball" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Badminton" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Table Tennis" });
            //context.Sports.AddOrUpdate(new Models.Sport { Description = "Softball" });
            context.SaveChanges();
        }
    }
}
