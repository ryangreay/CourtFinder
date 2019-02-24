namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
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
                        Team_TeamID = c.Int(),
                        WinningTeam_TeamID = c.Int(),
                        Bracket_BracketID = c.Int(),
                    })
                .PrimaryKey(t => t.GameID)
                .ForeignKey("dbo.Teams", t => t.Team_TeamID)
                .ForeignKey("dbo.Courts", t => t.CourtID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.WinningTeam_TeamID)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID)
                .Index(t => t.CourtID)
                .Index(t => t.Team_TeamID)
                .Index(t => t.WinningTeam_TeamID)
                .Index(t => t.Bracket_BracketID);
            
            CreateTable(
                "dbo.Courts",
                c => new
                    {
                        CourtID = c.Int(nullable: false, identity: true),
                        FacilityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CourtID)
                .ForeignKey("dbo.Facilities", t => t.FacilityID, cascadeDelete: true)
                .Index(t => t.FacilityID);
            
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
                "dbo.Sports",
                c => new
                    {
                        SportID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        SportImageURL = c.String(),
                    })
                .PrimaryKey(t => t.SportID);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        PlayerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        BirthDate = c.DateTime(),
                        ProfileImage = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Gender = c.String(),
                        Height = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        Sport_SportID = c.Int(),
                    })
                .PrimaryKey(t => t.PlayerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .ForeignKey("dbo.Sports", t => t.Sport_SportID)
                .Index(t => t.UserID)
                .Index(t => t.Sport_SportID);
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        TeamID = c.Int(nullable: false, identity: true),
                        PrivateTeamID = c.String(),
                        TeamName = c.String(),
                        Wins = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        Standing = c.Double(nullable: false),
                        Game_GameID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamID)
                .ForeignKey("dbo.Games", t => t.Game_GameID)
                .Index(t => t.Game_GameID);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.FacilityManagers",
                c => new
                    {
                        FacilityManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        ProfileImage = c.Binary(),
                    })
                .PrimaryKey(t => t.FacilityManagerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.FacilityManagers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.Facilities", "FacilityManager_FacilityManagerID", "dbo.FacilityManagers");
            DropForeignKey("dbo.Games", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Games", "WinningTeam_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Game_GameID", "dbo.Games");
            DropForeignKey("dbo.Games", "CourtID", "dbo.Courts");
            DropForeignKey("dbo.Leagues", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.Leagues", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamPlayers", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.TeamPlayers", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.TeamLeagues", "League_LeagueID", "dbo.Leagues");
            DropForeignKey("dbo.TeamLeagues", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Leagues", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Courts", "FacilityID", "dbo.Facilities");
            DropIndex("dbo.TeamPlayers", new[] { "Player_PlayerID" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_TeamID" });
            DropIndex("dbo.TeamLeagues", new[] { "League_LeagueID" });
            DropIndex("dbo.TeamLeagues", new[] { "Team_TeamID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.FacilityManagers", new[] { "UserID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Teams", new[] { "Game_GameID" });
            DropIndex("dbo.Players", new[] { "Sport_SportID" });
            DropIndex("dbo.Players", new[] { "UserID" });
            DropIndex("dbo.Leagues", new[] { "Facility_FacilityID" });
            DropIndex("dbo.Leagues", new[] { "Sport_SportID" });
            DropIndex("dbo.Leagues", new[] { "BracketID" });
            DropIndex("dbo.Facilities", new[] { "FacilityManager_FacilityManagerID" });
            DropIndex("dbo.Courts", new[] { "FacilityID" });
            DropIndex("dbo.Games", new[] { "Bracket_BracketID" });
            DropIndex("dbo.Games", new[] { "WinningTeam_TeamID" });
            DropIndex("dbo.Games", new[] { "Team_TeamID" });
            DropIndex("dbo.Games", new[] { "CourtID" });
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.TeamLeagues");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.FacilityManagers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Teams");
            DropTable("dbo.Players");
            DropTable("dbo.Sports");
            DropTable("dbo.Leagues");
            DropTable("dbo.Facilities");
            DropTable("dbo.Courts");
            DropTable("dbo.Games");
            DropTable("dbo.Brackets");
        }
    }
}
