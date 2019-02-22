namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Facilityintegration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Courts", "Owner_CourtOwnerID", "dbo.CourtOwners");
            DropForeignKey("dbo.CourtOwners", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Events", "Coordinator_CourtOwnerID", "dbo.CourtOwners");
            DropForeignKey("dbo.Events", "Court_CourtID", "dbo.Courts");
            DropForeignKey("dbo.PlayerSports", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.PlayerSports", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Events", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Events", "Winner_TeamID", "dbo.Teams");
            DropIndex("dbo.Courts", new[] { "Owner_CourtOwnerID" });
            DropIndex("dbo.Events", new[] { "Coordinator_CourtOwnerID" });
            DropIndex("dbo.Events", new[] { "Court_CourtID" });
            DropIndex("dbo.Events", new[] { "Team_TeamID" });
            DropIndex("dbo.Events", new[] { "Sport_SportID" });
            DropIndex("dbo.Events", new[] { "Winner_TeamID" });
            DropIndex("dbo.CourtOwners", new[] { "UserID" });
            DropIndex("dbo.PlayerSports", new[] { "Player_PlayerID" });
            DropIndex("dbo.PlayerSports", new[] { "Sport_SportID" });
            CreateTable(
                "dbo.Brackets",
                c => new
                    {
                        BracketID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.BracketID);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        GameStart = c.DateTime(nullable: false),
                        GameEnd = c.DateTime(nullable: false),
                        CourtID = c.Int(nullable: false),
                        TeamID = c.Int(nullable: false),
                        Bracket_BracketID = c.Int(),
                    })
                .PrimaryKey(t => t.GameID)
                .ForeignKey("dbo.Courts", t => t.CourtID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID)
                .Index(t => t.CourtID)
                .Index(t => t.TeamID)
                .Index(t => t.Bracket_BracketID);
            
            CreateTable(
                "dbo.Facilities",
                c => new
                    {
                        FacilityID = c.Int(nullable: false, identity: true),
                        FacilityName = c.String(),
                        Latitude = c.String(),
                        Longitude = c.String(),
                        FacilityManager_FacilityManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.FacilityID)
                .ForeignKey("dbo.FacilityManagers", t => t.FacilityManager_FacilityManagerID)
                .Index(t => t.FacilityManager_FacilityManagerID);
            
            CreateTable(
                "dbo.Leagues",
                c => new
                    {
                        LeagueID = c.Int(nullable: false, identity: true),
                        RegisterStartPeriod = c.DateTime(nullable: false),
                        RegisterEndPeriod = c.DateTime(nullable: false),
                        TeamSize = c.Int(nullable: false),
                        MinTeams = c.Int(nullable: false),
                        MaxTeams = c.Int(nullable: false),
                        BracketID = c.Int(nullable: false),
                        Sport_SportID = c.Int(),
                        Facility_FacilityID = c.Int(),
                    })
                .PrimaryKey(t => t.LeagueID)
                .ForeignKey("dbo.Brackets", t => t.BracketID, cascadeDelete: true)
                .ForeignKey("dbo.Sports", t => t.Sport_SportID)
                .ForeignKey("dbo.Facilities", t => t.Facility_FacilityID)
                .Index(t => t.BracketID)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Facility_FacilityID);
            
            CreateTable(
                "dbo.FacilityManagers",
                c => new
                    {
                        FacilityManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        ManagerTitle = c.String(),
                    })
                .PrimaryKey(t => t.FacilityManagerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.TeamLeagues",
                c => new
                    {
                        Team_TeamID = c.Int(nullable: false),
                        League_LeagueID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Team_TeamID, t.League_LeagueID })
                .ForeignKey("dbo.Teams", t => t.Team_TeamID, cascadeDelete: true)
                .ForeignKey("dbo.Leagues", t => t.League_LeagueID, cascadeDelete: true)
                .Index(t => t.Team_TeamID)
                .Index(t => t.League_LeagueID);

            CreateTable(
                "dbo.TeamGames",
                c => new
                {
                    Team_TeamID = c.Int(nullable: false),
                    Game_GameID = c.Int(nullable: false),
                })
                .PrimaryKey(t => new { t.Team_TeamID, t.Game_GameID })
                .ForeignKey("dbo.Teams", t => t.Team_TeamID)
                .ForeignKey("dbo.Games", t => t.Game_GameID)
                .Index(t => t.Team_TeamID)
                .Index(t => t.Game_GameID);

            AddColumn("dbo.Courts", "FacilityID", c => c.Int(nullable: false));
            AddColumn("dbo.Players", "FullName", c => c.String());
            AddColumn("dbo.Players", "Sport_SportID", c => c.Int());
            AddColumn("dbo.Teams", "PrivateTeamID", c => c.String());
            CreateIndex("dbo.Courts", "FacilityID");
            CreateIndex("dbo.Players", "Sport_SportID");
            AddForeignKey("dbo.Courts", "FacilityID", "dbo.Facilities", "FacilityID", cascadeDelete: true);
            AddForeignKey("dbo.Players", "Sport_SportID", "dbo.Sports", "SportID");
            DropColumn("dbo.Courts", "Latitude");
            DropColumn("dbo.Courts", "Longitude");
            DropColumn("dbo.Courts", "Address");
            DropColumn("dbo.Courts", "Status");
            DropColumn("dbo.Courts", "CurrentOccupancy");
            DropColumn("dbo.Courts", "Owner_CourtOwnerID");
            DropColumn("dbo.Players", "MiddleName");
            
            DropTable("dbo.CourtOwners");
            DropTable("dbo.PlayerSports");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PlayerSports",
                c => new
                    {
                        Player_PlayerID = c.Int(nullable: false),
                        Sport_SportID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Player_PlayerID, t.Sport_SportID });
            
            CreateTable(
                "dbo.CourtOwners",
                c => new
                    {
                        CourtOwnerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        Title = c.String(),
                    })
                .PrimaryKey(t => t.CourtOwnerID);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        EventID = c.Int(nullable: false, identity: true),
                        EventStart = c.DateTime(nullable: false),
                        EventEnd = c.DateTime(nullable: false),
                        CourtNumber = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Coordinator_CourtOwnerID = c.Int(),
                        Court_CourtID = c.Int(),
                        Team_TeamID = c.Int(),
                        Sport_SportID = c.Int(),
                        Winner_TeamID = c.Int(),
                    })
                .PrimaryKey(t => t.EventID);
            
            AddColumn("dbo.Teams", "Event_EventID", c => c.Int());
            AddColumn("dbo.Players", "MiddleName", c => c.String());
            AddColumn("dbo.Courts", "Owner_CourtOwnerID", c => c.Int());
            AddColumn("dbo.Courts", "CurrentOccupancy", c => c.Int(nullable: false));
            AddColumn("dbo.Courts", "Status", c => c.Boolean(nullable: false));
            AddColumn("dbo.Courts", "Address", c => c.String());
            AddColumn("dbo.Courts", "Longitude", c => c.Double(nullable: false));
            AddColumn("dbo.Courts", "Latitude", c => c.Double(nullable: false));
            DropForeignKey("dbo.FacilityManagers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Facilities", "FacilityManager_FacilityManagerID", "dbo.FacilityManagers");
            DropForeignKey("dbo.Games", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Games", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Game_GameID", "dbo.Games");
            DropForeignKey("dbo.Games", "CourtID", "dbo.Courts");
            DropForeignKey("dbo.Leagues", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.Leagues", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.TeamLeagues", "League_LeagueID", "dbo.Leagues");
            DropForeignKey("dbo.TeamLeagues", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Courts", "FacilityID", "dbo.Facilities");
            DropIndex("dbo.TeamLeagues", new[] { "League_LeagueID" });
            DropIndex("dbo.TeamLeagues", new[] { "Team_TeamID" });
            DropIndex("dbo.FacilityManagers", new[] { "UserID" });
            DropIndex("dbo.Teams", new[] { "Game_GameID" });
            DropIndex("dbo.Players", new[] { "Sport_SportID" });
            DropIndex("dbo.Leagues", new[] { "Facility_FacilityID" });
            DropIndex("dbo.Leagues", new[] { "Sport_SportID" });
            DropIndex("dbo.Leagues", new[] { "BracketID" });
            DropIndex("dbo.Facilities", new[] { "FacilityManager_FacilityManagerID" });
            DropIndex("dbo.Courts", new[] { "FacilityID" });
            DropIndex("dbo.Games", new[] { "Bracket_BracketID" });
            DropIndex("dbo.Games", new[] { "Team_TeamID" });
            DropIndex("dbo.Games", new[] { "TeamID" });
            DropIndex("dbo.Games", new[] { "CourtID" });
            DropColumn("dbo.Teams", "Game_GameID");
            DropColumn("dbo.Teams", "PrivateTeamID");
            DropColumn("dbo.Players", "Sport_SportID");
            DropColumn("dbo.Players", "FullName");
            DropColumn("dbo.Courts", "FacilityID");
            DropTable("dbo.TeamLeagues");
            DropTable("dbo.FacilityManagers");
            DropTable("dbo.Leagues");
            DropTable("dbo.Facilities");
            DropTable("dbo.Games");
            DropTable("dbo.Brackets");
            CreateIndex("dbo.PlayerSports", "Sport_SportID");
            CreateIndex("dbo.PlayerSports", "Player_PlayerID");
            CreateIndex("dbo.Teams", "Event_EventID");
            CreateIndex("dbo.CourtOwners", "UserID");
            CreateIndex("dbo.Events", "Winner_TeamID");
            CreateIndex("dbo.Events", "Sport_SportID");
            CreateIndex("dbo.Events", "Team_TeamID");
            CreateIndex("dbo.Events", "Court_CourtID");
            CreateIndex("dbo.Events", "Coordinator_CourtOwnerID");
            CreateIndex("dbo.Courts", "Owner_CourtOwnerID");
            AddForeignKey("dbo.Events", "Winner_TeamID", "dbo.Teams", "TeamID");
            AddForeignKey("dbo.Teams", "Event_EventID", "dbo.Events", "EventID");
            AddForeignKey("dbo.Events", "Sport_SportID", "dbo.Sports", "SportID");
            AddForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams", "TeamID");
            AddForeignKey("dbo.PlayerSports", "Sport_SportID", "dbo.Sports", "SportID", cascadeDelete: true);
            AddForeignKey("dbo.PlayerSports", "Player_PlayerID", "dbo.Players", "PlayerID", cascadeDelete: true);
            AddForeignKey("dbo.Events", "Court_CourtID", "dbo.Courts", "CourtID");
            AddForeignKey("dbo.Events", "Coordinator_CourtOwnerID", "dbo.CourtOwners", "CourtOwnerID");
            AddForeignKey("dbo.CourtOwners", "UserID", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Courts", "Owner_CourtOwnerID", "dbo.CourtOwners", "CourtOwnerID");
        }
    }
}
