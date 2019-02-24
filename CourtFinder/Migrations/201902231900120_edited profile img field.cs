namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class editedprofileimgfield : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Players", "ProfileImage", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Players", "ProfileImage", c => c.Binary());
        }
    }
}
