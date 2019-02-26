namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedfacilitylatlongtodouble : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Facilities", "Latitude", c => c.Double(nullable: false));
            AlterColumn("dbo.Facilities", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Facilities", "Longitude", c => c.Double());
            AlterColumn("dbo.Facilities", "Latitude", c => c.Double());
        }
    }
}
