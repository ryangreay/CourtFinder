namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
            
            CreateTable(
                "dbo.Brackets",
                c => new
                    {
                        BracketID = c.Int(nullable: false, identity: true),
                        BracketStartDate = c.DateTime(nullable: false),
                        GameLength = c.Time(nullable: false, precision: 7),
                        daysBetweenRounds = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BracketID);
            
            CreateTable(
                "dbo.Days",
                c => new
                    {
                        DayID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.DayID);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        GameID = c.Int(nullable: false, identity: true),
                        GameStart = c.DateTime(nullable: false),
                        GameEnd = c.DateTime(nullable: false),
                        CourtID = c.Int(nullable: false),
                        Team_TeamID = c.Int(),
                        Team1_TeamID = c.Int(),
                        Team2_TeamID = c.Int(),
                        WinningTeam_TeamID = c.Int(),
                        Bracket_BracketID = c.Int(),
                    })
                .PrimaryKey(t => t.GameID)
                .ForeignKey("dbo.Teams", t => t.Team_TeamID)
                .ForeignKey("dbo.Courts", t => t.CourtID, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team1_TeamID)
                .ForeignKey("dbo.Teams", t => t.Team2_TeamID)
                .ForeignKey("dbo.Teams", t => t.WinningTeam_TeamID)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID)
                .Index(t => t.CourtID)
                .Index(t => t.Team_TeamID)
                .Index(t => t.Team1_TeamID)
                .Index(t => t.Team2_TeamID)
                .Index(t => t.WinningTeam_TeamID)
                .Index(t => t.Bracket_BracketID);
            
            CreateTable(
                "dbo.Courts",
                c => new
                    {
                        CourtID = c.Int(nullable: false, identity: true),
                        FacilityID = c.Int(nullable: false),
                        CourtName = c.String(),
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
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                        Address = c.String(),
                        FacilityManager_FacilityManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.FacilityID)
                .ForeignKey("dbo.FacilityManagers", t => t.FacilityManager_FacilityManagerID)
                .Index(t => t.FacilityManager_FacilityManagerID);
            
            CreateTable(
                "dbo.FacilityManagers",
                c => new
                    {
                        FacilityManagerID = c.Int(nullable: false, identity: true),
                        UserID = c.String(maxLength: 128),
                        ProfileImage = c.String(),
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.FacilityManagerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);
            
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
                "dbo.Leagues",
                c => new
                    {
                        LeagueID = c.Int(nullable: false, identity: true),
                        LeagueName = c.String(),
                        RegisterStartPeriod = c.DateTime(nullable: false),
                        RegisterEndPeriod = c.DateTime(nullable: false),
                        TeamSize = c.Int(nullable: false),
                        MinTeams = c.Int(nullable: false),
                        MaxTeams = c.Int(nullable: false),
                        Bracket_BracketID = c.Int(),
                        Sport_SportID = c.Int(),
                        Facility_FacilityID = c.Int(),
                    })
                .PrimaryKey(t => t.LeagueID)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID)
                .ForeignKey("dbo.Sports", t => t.Sport_SportID)
                .ForeignKey("dbo.Facilities", t => t.Facility_FacilityID)
                .Index(t => t.Bracket_BracketID)
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
                        Latitude = c.Double(nullable: false),
                        Longitude = c.Double(nullable: false),
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
                        Bracket_BracketID = c.Int(),
                    })
                .PrimaryKey(t => t.TeamID)
                .ForeignKey("dbo.Brackets", t => t.Bracket_BracketID)
                .Index(t => t.Bracket_BracketID);
            
            CreateTable(
                "dbo.Times",
                c => new
                    {
                        TimeID = c.Int(nullable: false, identity: true),
                        Description = c.Time(nullable: false, precision: 7),
                    })
                .PrimaryKey(t => t.TimeID);
            
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
                "dbo.SportCourts",
                c => new
                    {
                        Sport_SportID = c.Int(nullable: false),
                        Court_CourtID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Sport_SportID, t.Court_CourtID })
                .ForeignKey("dbo.Sports", t => t.Sport_SportID, cascadeDelete: true)
                .ForeignKey("dbo.Courts", t => t.Court_CourtID, cascadeDelete: true)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Court_CourtID);
            
            CreateTable(
                "dbo.SportFacilities",
                c => new
                    {
                        Sport_SportID = c.Int(nullable: false),
                        Facility_FacilityID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Sport_SportID, t.Facility_FacilityID })
                .ForeignKey("dbo.Sports", t => t.Sport_SportID, cascadeDelete: true)
                .ForeignKey("dbo.Facilities", t => t.Facility_FacilityID, cascadeDelete: true)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Facility_FacilityID);
            
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
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.BracketResults", "TeamID", "dbo.Teams");
            DropForeignKey("dbo.BracketResults", "BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Teams", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.TimeBrackets", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.TimeBrackets", "Time_TimeID", "dbo.Times");
            DropForeignKey("dbo.Games", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Games", "WinningTeam_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team2_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team1_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "CourtID", "dbo.Courts");
            DropForeignKey("dbo.Leagues", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.Leagues", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamPlayers", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.TeamPlayers", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.TeamLeagues", "League_LeagueID", "dbo.Leagues");
            DropForeignKey("dbo.TeamLeagues", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.SportFacilities", "Facility_FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.SportFacilities", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.SportCourts", "Court_CourtID", "dbo.Courts");
            DropForeignKey("dbo.SportCourts", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Leagues", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.Facilities", "FacilityManager_FacilityManagerID", "dbo.FacilityManagers");
            DropForeignKey("dbo.FacilityManagers", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courts", "FacilityID", "dbo.Facilities");
            DropForeignKey("dbo.DayBrackets", "Bracket_BracketID", "dbo.Brackets");
            DropForeignKey("dbo.DayBrackets", "Day_DayID", "dbo.Days");
            DropIndex("dbo.TimeBrackets", new[] { "Bracket_BracketID" });
            DropIndex("dbo.TimeBrackets", new[] { "Time_TimeID" });
            DropIndex("dbo.TeamPlayers", new[] { "Player_PlayerID" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_TeamID" });
            DropIndex("dbo.TeamLeagues", new[] { "League_LeagueID" });
            DropIndex("dbo.TeamLeagues", new[] { "Team_TeamID" });
            DropIndex("dbo.SportFacilities", new[] { "Facility_FacilityID" });
            DropIndex("dbo.SportFacilities", new[] { "Sport_SportID" });
            DropIndex("dbo.SportCourts", new[] { "Court_CourtID" });
            DropIndex("dbo.SportCourts", new[] { "Sport_SportID" });
            DropIndex("dbo.DayBrackets", new[] { "Bracket_BracketID" });
            DropIndex("dbo.DayBrackets", new[] { "Day_DayID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Teams", new[] { "Bracket_BracketID" });
            DropIndex("dbo.Players", new[] { "Sport_SportID" });
            DropIndex("dbo.Players", new[] { "UserID" });
            DropIndex("dbo.Leagues", new[] { "Facility_FacilityID" });
            DropIndex("dbo.Leagues", new[] { "Sport_SportID" });
            DropIndex("dbo.Leagues", new[] { "Bracket_BracketID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.FacilityManagers", new[] { "UserID" });
            DropIndex("dbo.Facilities", new[] { "FacilityManager_FacilityManagerID" });
            DropIndex("dbo.Courts", new[] { "FacilityID" });
            DropIndex("dbo.Games", new[] { "Bracket_BracketID" });
            DropIndex("dbo.Games", new[] { "WinningTeam_TeamID" });
            DropIndex("dbo.Games", new[] { "Team2_TeamID" });
            DropIndex("dbo.Games", new[] { "Team1_TeamID" });
            DropIndex("dbo.Games", new[] { "Team_TeamID" });
            DropIndex("dbo.Games", new[] { "CourtID" });
            DropIndex("dbo.BracketResults", new[] { "TeamID" });
            DropIndex("dbo.BracketResults", new[] { "BracketID" });
            DropTable("dbo.TimeBrackets");
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.TeamLeagues");
            DropTable("dbo.SportFacilities");
            DropTable("dbo.SportCourts");
            DropTable("dbo.DayBrackets");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Times");
            DropTable("dbo.Teams");
            DropTable("dbo.Players");
            DropTable("dbo.Sports");
            DropTable("dbo.Leagues");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.FacilityManagers");
            DropTable("dbo.Facilities");
            DropTable("dbo.Courts");
            DropTable("dbo.Games");
            DropTable("dbo.Days");
            DropTable("dbo.Brackets");
            DropTable("dbo.BracketResults");
        }
    }
}
