namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateleaguebracketrelation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets");
            DropIndex("dbo.Leagues", new[] { "BracketID" });
            RenameColumn(table: "dbo.Leagues", name: "BracketID", newName: "Bracket_BracketID");
            AlterColumn("dbo.Leagues", "Bracket_BracketID", c => c.Int());
            CreateIndex("dbo.Leagues", "Bracket_BracketID");
            AddForeignKey("dbo.Leagues", "Bracket_BracketID", "dbo.Brackets", "BracketID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Leagues", "Bracket_BracketID", "dbo.Brackets");
            DropIndex("dbo.Leagues", new[] { "Bracket_BracketID" });
            AlterColumn("dbo.Leagues", "Bracket_BracketID", c => c.Int(nullable: false));
            RenameColumn(table: "dbo.Leagues", name: "Bracket_BracketID", newName: "BracketID");
            CreateIndex("dbo.Leagues", "BracketID");
            AddForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets", "BracketID", cascadeDelete: true);
        }
    }
}
