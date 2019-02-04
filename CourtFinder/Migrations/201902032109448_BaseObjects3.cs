namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseObjects3 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamEvents",
                c => new
                {
                    Team_TeamID = c.Int(nullable: false),
                    Event_EventID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Team_TeamID, t.Event_EventID })
                .ForeignKey("dbo.Teams", t => t.Team_TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.Event_EventID, cascadeDelete: true)
                .Index(t => t.Team_TeamID)
                .Index(t => t.Event_EventID);
            DropForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Event_EventID", "dbo.Events");
            DropIndex("dbo.Events", new[] { "Team_TeamID" });
            DropIndex("dbo.Teams", new[] { "Event_EventID" });
            DropColumn("dbo.Events", "Team_TeamID");
            DropColumn("dbo.Teams", "Event_EventID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "Team_TeamID", c => c.Int());
            AddColumn("dbo.Teams", "Event_EventID", c => c.Int());
            CreateIndex("dbo.Events", new[] { "Team_TeamID" });
            CreateIndex("dbo.Teams", new[] { "Event_EventID" });
            AddForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            AddForeignKey("dbo.Teams", "Event_EventID", "dbo.Events");           
        }
    }
}
