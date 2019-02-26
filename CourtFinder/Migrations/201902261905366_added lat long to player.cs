namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedlatlongtoplayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "Latitude", c => c.Double(nullable: false));
            AddColumn("dbo.Players", "Longitude", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Players", "Longitude");
            DropColumn("dbo.Players", "Latitude");
        }
    }
}
