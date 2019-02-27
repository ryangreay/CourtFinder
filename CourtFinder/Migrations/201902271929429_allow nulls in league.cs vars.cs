namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allownullsinleaguecsvars : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets");
            DropIndex("dbo.Leagues", new[] { "BracketID" });
            AddColumn("dbo.Leagues", "EnrolledTeams", c => c.Int(nullable: false));
            AlterColumn("dbo.Leagues", "RegisterEndPeriod", c => c.DateTime());
            AlterColumn("dbo.Leagues", "BracketID", c => c.Int());
            CreateIndex("dbo.Leagues", "BracketID");
            AddForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets", "BracketID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets");
            DropIndex("dbo.Leagues", new[] { "BracketID" });
            AlterColumn("dbo.Leagues", "BracketID", c => c.Int(nullable: false));
            AlterColumn("dbo.Leagues", "RegisterEndPeriod", c => c.DateTime(nullable: false));
            DropColumn("dbo.Leagues", "EnrolledTeams");
            CreateIndex("dbo.Leagues", "BracketID");
            AddForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets", "BracketID", cascadeDelete: true);
        }
    }
}
