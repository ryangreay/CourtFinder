namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedleaguename : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Leagues", "LeagueName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Leagues", "LeagueName");
        }
    }
}
