namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class manytomanyonplayersandfacilities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlayerFacilities",
                c => new
                    {
                        Player_PlayerID = c.Int(nullable: false),
                        Facility_FacilityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Player_PlayerID, t.Facility_FacilityID })
                .ForeignKey("dbo.Players", t => t.Player_PlayerID, cascadeDelete: true)
                .ForeignKey("dbo.Facilities", t => t.Facility_FacilityID, cascadeDelete: true)
                .Index(t => t.Player_PlayerID)
                .Index(t => t.Facility_FacilityID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlayerFacilities", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.PlayerFacilities", "Player_PlayerID", "dbo.Players");
            DropIndex("dbo.PlayerFacilities", new[] { "Facility_FacilityID" });
            DropIndex("dbo.PlayerFacilities", new[] { "Player_PlayerID" });
            DropTable("dbo.PlayerFacilities");
        }
    }
}
