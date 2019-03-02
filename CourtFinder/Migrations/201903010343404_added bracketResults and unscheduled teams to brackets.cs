namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedbracketResultsandunscheduledteamstobrackets : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BracketResults",
                c => new
                    {
                        BracketResultID = c.Int(nullable: false, identity: true),
                        BracketID = c.Int(nullable: false),
                        TeamID = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BracketResultID)
                .ForeignKey("dbo.Brackets", t => t.BracketID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .Index(t => t.BracketID)
                .Index(t => t.TeamID);
            
            AddColumn("dbo.Teams", "Bracket_BracketID", c => c.Int());
            CreateIndex("dbo.Teams", "Bracket_BracketID");
            AddForeignKey("dbo.Teams", "Bracket_BracketID", "dbo.Brackets", "BracketID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BracketResults", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.BracketResults", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Teams", "Bracket_BracketID", "dbo.Brackets");
            DropIndex("dbo.Teams", new[] { "Bracket_BracketID" });
            DropIndex("dbo.BracketResults", new[] { "TeamID" });
            DropIndex("dbo.BracketResults", new[] { "BracketID" });
            DropColumn("dbo.Teams", "Bracket_BracketID");
            DropTable("dbo.BracketResults");
        }
    }
}
