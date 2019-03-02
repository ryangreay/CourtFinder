namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class sporttblchange2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sports", "Court_CourtID", "dbo.Courts");
            DropIndex("dbo.Sports", new[] { "Court_CourtID" });
            CreateTable(
                "dbo.SportCourts",
                c => new
                    {
                        Sport_SportID = c.Int(nullable: false),
                        Court_CourtID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Sport_SportID, t.Court_CourtID })
                .ForeignKey("dbo.Sports", t => t.Sport_SportID, cascadeDelete: true)
                .ForeignKey("dbo.Courts", t => t.Court_CourtID, cascadeDelete: true)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Court_CourtID);
            
            DropColumn("dbo.Sports", "Court_CourtID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Sports", "Court_CourtID", c => c.Int());
            DropForeignKey("dbo.SportCourts", "Court_CourtID", "dbo.Courts");
            DropForeignKey("dbo.SportCourts", "Sport_SportID", "dbo.Sports");
            DropIndex("dbo.SportCourts", new[] { "Court_CourtID" });
            DropIndex("dbo.SportCourts", new[] { "Sport_SportID" });
            DropTable("dbo.SportCourts");
            CreateIndex("dbo.Sports", "Court_CourtID");
            AddForeignKey("dbo.Sports", "Court_CourtID", "dbo.Courts", "CourtID");
        }
    }
}
