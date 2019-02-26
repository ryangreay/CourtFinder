namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedsportslisttofacility : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SportFacilities",
                c => new
                    {
                        Sport_SportID = c.Int(nullable: false),
                        Facility_FacilityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Sport_SportID, t.Facility_FacilityID })
                .ForeignKey("dbo.Sports", t => t.Sport_SportID, cascadeDelete: true)
                .ForeignKey("dbo.Facilities", t => t.Facility_FacilityID, cascadeDelete: true)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Facility_FacilityID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SportFacilities", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.SportFacilities", "Sport_SportID", "dbo.Sports");
            DropIndex("dbo.SportFacilities", new[] { "Facility_FacilityID" });
            DropIndex("dbo.SportFacilities", new[] { "Sport_SportID" });
            DropTable("dbo.SportFacilities");
        }
    }
}
