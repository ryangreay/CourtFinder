namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BaseObjects2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sports", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.Players", "Team_TeamID", "dbo.Teams");
            DropIndex("dbo.Sports", new[] { "Player_PlayerID" });
            DropIndex("dbo.Players", new[] { "Team_TeamID" });
            RenameColumn(table: "dbo.Courts", name: "CourtOwner_CourtOwnerID", newName: "Owner_CourtOwnerID");
            RenameIndex(table: "dbo.Courts", name: "IX_CourtOwner_CourtOwnerID", newName: "IX_Owner_CourtOwnerID");
            CreateTable(
                "dbo.PlayerSports",
                c => new
                    {
                        Player_PlayerID = c.Int(nullable: false),
                        Sport_SportID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Player_PlayerID, t.Sport_SportID })
                .ForeignKey("dbo.Players", t => t.Player_PlayerID, cascadeDelete: true)
                .ForeignKey("dbo.Sports", t => t.Sport_SportID, cascadeDelete: true)
                .Index(t => t.Player_PlayerID)
                .Index(t => t.Sport_SportID);
            
            CreateTable(
                "dbo.TeamPlayers",
                c => new
                    {
                        Team_TeamID = c.Int(nullable: false),
                        Player_PlayerID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Team_TeamID, t.Player_PlayerID })
                .ForeignKey("dbo.Teams", t => t.Team_TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Players", t => t.Player_PlayerID, cascadeDelete: true)
                .Index(t => t.Team_TeamID)
                .Index(t => t.Player_PlayerID);
            
            DropColumn("dbo.Sports", "Player_PlayerID");
            DropColumn("dbo.Players", "Team_TeamID");

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
            AddColumn("dbo.Players", "Team_TeamID", c => c.Int());
            AddColumn("dbo.Sports", "Player_PlayerID", c => c.Int());
            DropForeignKey("dbo.TeamPlayers", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.TeamPlayers", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.PlayerSports", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.PlayerSports", "Player_PlayerID", "dbo.Players");
            DropIndex("dbo.TeamPlayers", new[] { "Player_PlayerID" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_TeamID" });
            DropIndex("dbo.PlayerSports", new[] { "Sport_SportID" });
            DropIndex("dbo.PlayerSports", new[] { "Player_PlayerID" });
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.PlayerSports");
            RenameIndex(table: "dbo.Courts", name: "IX_Owner_CourtOwnerID", newName: "IX_CourtOwner_CourtOwnerID");
            RenameColumn(table: "dbo.Courts", name: "Owner_CourtOwnerID", newName: "CourtOwner_CourtOwnerID");
            CreateIndex("dbo.Players", "Team_TeamID");
            CreateIndex("dbo.Sports", "Player_PlayerID");
            AddForeignKey("dbo.Players", "Team_TeamID", "dbo.Teams", "TeamID");
            AddForeignKey("dbo.Sports", "Player_PlayerID", "dbo.Players", "PlayerID");

            AddColumn("dbo.Events", "Team_TeamID", c => c.Int());
            AddColumn("dbo.Teams", "Event_EventID", c => c.Int());
            CreateIndex("dbo.Events", new[] { "Team_TeamID" });
            CreateIndex("dbo.Teams", new[] { "Event_EventID" });
            AddForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            AddForeignKey("dbo.Teams", "Event_EventID", "dbo.Events");
        }
    }
}
