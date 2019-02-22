namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facilitymanagerchange : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.FacilityManagers", "ManagerTitle");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FacilityManagers", "ManagerTitle", c => c.String());
        }
    }
}
