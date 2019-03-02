namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addsportstocourtsandgamelengthtobracket : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Brackets", "BracketStartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Brackets", "GameLength", c => c.Time(nullable: false, precision: 7));
            AddColumn("dbo.Sports", "Court_CourtID", c => c.Int());
            CreateIndex("dbo.Sports", "Court_CourtID");
            AddForeignKey("dbo.Sports", "Court_CourtID", "dbo.Courts", "CourtID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Sports", "Court_CourtID", "dbo.Courts");
            DropIndex("dbo.Sports", new[] { "Court_CourtID" });
            DropColumn("dbo.Sports", "Court_CourtID");
            DropColumn("dbo.Brackets", "GameLength");
            DropColumn("dbo.Brackets", "BracketStartDate");
        }
    }
}
