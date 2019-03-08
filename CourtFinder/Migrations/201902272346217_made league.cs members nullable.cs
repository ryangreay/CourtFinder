namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madeleaguecsmembersnullable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courts", "CourtName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courts", "CourtName");
        }
    }
}
