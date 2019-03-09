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
            /*
            context.Days.AddOrUpdate(new Models.Day {Description = "Monday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Tuesday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Wednesday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Thursday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Friday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Saturday" });
            context.Days.AddOrUpdate(new Models.Day { Description = "Sunday" });
            for (int hour = 0; hour < (24); hour++) //24 hours in a day, 6 10 minute intervals every hour
            {
                for (int minute = 0; minute < 6; minute++)
                context.Times.AddOrUpdate(new Models.Time { Description = new TimeSpan(hour, minute * 10, 0)});
            }

            context.Roles.AddOrUpdate(new IdentityRole { Name = "Player" });
            context.Roles.AddOrUpdate(new IdentityRole { Name = "FacilityManager" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Basketball" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Baseball" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Soccer" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Volleyball" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Badminton" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Table Tennis" });
            context.Sports.AddOrUpdate(new Models.Sport { Description = "Softball" });
            context.SaveChanges();
            */
        }
    }
}
