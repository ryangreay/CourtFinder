namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseObjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Courts",
                c => new
                    {
                        CourtID = c.Int(nullable: false, identity: true),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Address = c.String(),
                        Status = c.Boolean(nullable: false),
                        CurrentOccupancy = c.Int(nullable: false),
                        CourtOwner_CourtOwnerID = c.Int(),
                    })
                .PrimaryKey(t => t.CourtID)
                .ForeignKey("dbo.CourtOwners", t => t.CourtOwner_CourtOwnerID)
                .Index(t => t.CourtOwner_CourtOwnerID);
            
            CreateTable(
                "dbo.CourtOwners",
                c => new
                    {
                        CourtOwnerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.CourtOwnerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Sports",
                c => new
                    {
                        SportID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        SportImageURL = c.String(),
                        Player_PlayerID = c.Int(),
                    })
                .PrimaryKey(t => t.SportID)
                .ForeignKey("dbo.Players", t => t.Player_PlayerID)
                .Index(t => t.Player_PlayerID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        TeamName = c.String(),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        Event_EventID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamID)
                .ForeignKey("dbo.Events", t => t.Event_EventID)
                .Index(t => t.Event_EventID);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        BirthDate = c.DateTime(nullable: false),
                        Gender = c.String(),
                        Height = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Team_TeamID = c.Int(),
                    })
                .PrimaryKey(t => t.PlayerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Teams", t => t.Team_TeamID)
                .Index(t => t.UserID)
                .Index(t => t.Team_TeamID);
            
            AddColumn("dbo.Events", "EventStart", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "EventEnd", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "CourtNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "Capacity", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "Coordinator_CourtOwnerID", c => c.Int());
            AddColumn("dbo.Events", "Sport_SportID", c => c.Int());
            AddColumn("dbo.Events", "Team_TeamID", c => c.Int());
            AddColumn("dbo.Events", "Winner_TeamID", c => c.Int());
            AddColumn("dbo.Events", "Court_CourtID", c => c.Int());
            CreateIndex("dbo.Events", "Coordinator_CourtOwnerID");
            CreateIndex("dbo.Events", "Sport_SportID");
            CreateIndex("dbo.Events", "Team_TeamID");
            CreateIndex("dbo.Events", "Winner_TeamID");
            CreateIndex("dbo.Events", "Court_CourtID");
            AddForeignKey("dbo.Events", "Coordinator_CourtOwnerID", "dbo.CourtOwners", "CourtOwnerID");
            AddForeignKey("dbo.Events", "Sport_SportID", "dbo.Sports", "SportID");
            AddForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams", "TeamID");
            AddForeignKey("dbo.Events", "Winner_TeamID", "dbo.Teams", "TeamID");
            AddForeignKey("dbo.Events", "Court_CourtID", "dbo.Courts", "CourtID");
            DropColumn("dbo.Events", "EventDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "EventDate", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Events", "Court_CourtID", "dbo.Courts");
            DropForeignKey("dbo.Events", "Winner_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Event_EventID", "dbo.Events");
            DropForeignKey("dbo.Players", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Players", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Sports", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Events", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Events", "Coordinator_CourtOwnerID", "dbo.CourtOwners");
            DropForeignKey("dbo.CourtOwners", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courts", "CourtOwner_CourtOwnerID", "dbo.CourtOwners");
            DropIndex("dbo.Players", new[] { "Team_TeamID" });
            DropIndex("dbo.Players", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "Event_EventID" });
            DropIndex("dbo.Sports", new[] { "Player_PlayerID" });
            DropIndex("dbo.CourtOwners", new[] { "UserID" });
            DropIndex("dbo.Events", new[] { "Court_CourtID" });
            DropIndex("dbo.Events", new[] { "Winner_TeamID" });
            DropIndex("dbo.Events", new[] { "Team_TeamID" });
            DropIndex("dbo.Events", new[] { "Sport_SportID" });
            DropIndex("dbo.Events", new[] { "Coordinator_CourtOwnerID" });
            DropIndex("dbo.Courts", new[] { "CourtOwner_CourtOwnerID" });
            DropColumn("dbo.Events", "Court_CourtID");
            DropColumn("dbo.Events", "Winner_TeamID");
            DropColumn("dbo.Events", "Team_TeamID");
            DropColumn("dbo.Events", "Sport_SportID");
            DropColumn("dbo.Events", "Coordinator_CourtOwnerID");
            DropColumn("dbo.Events", "Capacity");
            DropColumn("dbo.Events", "CourtNumber");
            DropColumn("dbo.Events", "EventEnd");
            DropColumn("dbo.Events", "EventStart");
            DropTable("dbo.Players");
            DropTable("dbo.Teams");
            DropTable("dbo.Sports");
            DropTable("dbo.CourtOwners");
            DropTable("dbo.Courts");
        }
    }
}
