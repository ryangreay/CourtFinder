namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedlatlongtofacilitymanager : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.FacilityManagers", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.FacilityManagers", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacilityManagers", "Longitude");
            DropColumn("dbo.FacilityManagers", "Latitude");
        }
    }
}
