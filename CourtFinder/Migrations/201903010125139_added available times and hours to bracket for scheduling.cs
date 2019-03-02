namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedavailabletimesandhourstobracketforscheduling : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        DayID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.DayID);
            
            CreateTable(
                "dbo.Times",
                c => new
                    {
                        TimeID = c.Int(nullable: false, identity: true),
                        Description = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.TimeID);
            
            CreateTable(
                "dbo.DayBrackets",
                c => new
                    {
                        Day_DayID = c.Int(nullable: false),
                        Bracket_BracketID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Day_DayID, t.Bracket_BracketID })
                .ForeignKey("dbo.Days", t => t.Day_DayID, cascadeDelete: true)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID, cascadeDelete: true)
                .Index(t => t.Day_DayID)
                .Index(t => t.Bracket_BracketID);
            
            CreateTable(
                "dbo.TimeBrackets",
                c => new
                    {
                        Time_TimeID = c.Int(nullable: false),
                        Bracket_BracketID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Time_TimeID, t.Bracket_BracketID })
                .ForeignKey("dbo.Times", t => t.Time_TimeID, cascadeDelete: true)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID, cascadeDelete: true)
                .Index(t => t.Time_TimeID)
                .Index(t => t.Bracket_BracketID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TimeBrackets", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.TimeBrackets", "Time_TimeID", "dbo.Times");
            DropForeignKey("dbo.DayBrackets", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.DayBrackets", "Day_DayID", "dbo.Days");
            DropIndex("dbo.TimeBrackets", new[] { "Bracket_BracketID" });
            DropIndex("dbo.TimeBrackets", new[] { "Time_TimeID" });
            DropIndex("dbo.DayBrackets", new[] { "Bracket_BracketID" });
            DropIndex("dbo.DayBrackets", new[] { "Day_DayID" });
            DropTable("dbo.TimeBrackets");
            DropTable("dbo.DayBrackets");
            DropTable("dbo.Times");
            DropTable("dbo.Days");
        }
    }
}
