namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allownullsonplayerbdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Players", "BirthDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Players", "BirthDate", c => c.DateTime(nullable: false));
        }
    }
}
