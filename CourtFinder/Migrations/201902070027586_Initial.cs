namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
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
                        Owner_CourtOwnerID = c.Int(),
                    })
                .PrimaryKey(t => t.CourtID)
                .ForeignKey("dbo.CourtOwners", t => t.Owner_CourtOwnerID)
                .Index(t => t.Owner_CourtOwnerID);
            
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
                        Sport_SportID = c.Int(),
                        Winner_TeamID = c.Int(),
                    })
                .PrimaryKey(t => t.EventID)
                .ForeignKey("dbo.CourtOwners", t => t.Coordinator_CourtOwnerID)
                .ForeignKey("dbo.Courts", t => t.Court_CourtID)
                .ForeignKey("dbo.Sports", t => t.Sport_SportID)
                .ForeignKey("dbo.Teams", t => t.Winner_TeamID)
                .Index(t => t.Coordinator_CourtOwnerID)
                .Index(t => t.Court_CourtID)
                .Index(t => t.Sport_SportID)
                .Index(t => t.Winner_TeamID);
            
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
                        BirthDate = c.DateTime(nullable: false),
                        Gender = c.String(),
                        Height = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        MiddleName = c.String(),
                    })
                .PrimaryKey(t => t.PlayerID)
                .ForeignKey("dbo.AspNetUsers", t => t.UserID)
                .Index(t => t.UserID);

            CreateTable(
                "dbo.Teams",
                c => new
                {
                    TeamID = c.Int(nullable: false, identity: true),
                    TeamName = c.String(),
                    Wins = c.Int(nullable: false),
                    Losses = c.Int(nullable: false),
                    Standing = c.Double(nullable: false),
                })
                .PrimaryKey(t => t.TeamID);
            
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

        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Events", "Winner_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Teams", "Event_EventID", "dbo.Events");
            DropForeignKey("dbo.Events", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.Players", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamPlayers", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.TeamPlayers", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.TeamEvents", "Event_EventID", "dbo.Events");
            DropForeignKey("dbo.TeamEvents", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.Events", "Team_TeamID", "dbo.Teams");
            DropForeignKey("dbo.PlayerSports", "Sport_SportID", "dbo.Sports");
            DropForeignKey("dbo.PlayerSports", "Player_PlayerID", "dbo.Players");
            DropForeignKey("dbo.Events", "Court_CourtID", "dbo.Courts");
            DropForeignKey("dbo.Events", "Coordinator_CourtOwnerID", "dbo.CourtOwners");
            DropForeignKey("dbo.CourtOwners", "UserID", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Courts", "Owner_CourtOwnerID", "dbo.CourtOwners");
            DropIndex("dbo.TeamPlayers", new[] { "Player_PlayerID" });
            DropIndex("dbo.TeamPlayers", new[] { "Team_TeamID" });
            DropIndex("dbo.TeamEvents", new[] { "Event_EventID" });
            DropIndex("dbo.TeamEvents", new[] { "Team_TeamID" });
            DropIndex("dbo.PlayerSports", new[] { "Sport_SportID" });
            DropIndex("dbo.PlayerSports", new[] { "Player_PlayerID" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Teams", new[] { "Event_EventID" });
            DropIndex("dbo.Players", new[] { "UserID" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.CourtOwners", new[] { "UserID" });
            DropIndex("dbo.Events", new[] { "Winner_TeamID" });
            DropIndex("dbo.Events", new[] { "Sport_SportID" });
            DropIndex("dbo.Events", new[] { "Team_TeamID" });
            DropIndex("dbo.Events", new[] { "Court_CourtID" });
            DropIndex("dbo.Events", new[] { "Coordinator_CourtOwnerID" });
            DropIndex("dbo.Courts", new[] { "Owner_CourtOwnerID" });
            DropTable("dbo.TeamPlayers");
            DropTable("dbo.TeamEvents");
            DropTable("dbo.PlayerSports");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Teams");
            DropTable("dbo.Players");
            DropTable("dbo.Sports");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.CourtOwners");
            DropTable("dbo.Events");
            DropTable("dbo.Courts");
        }
    }
}
