namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedprofileimagefieldtoplayer : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "ProfileImage", c => c.Binary());
            AddColumn("dbo.FacilityManagers", "ProfileImage", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.FacilityManagers", "ProfileImage");
            DropColumn("dbo.Players", "ProfileImage");
        }
    }
}
