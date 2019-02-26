namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedaddressfieldtofacility : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Facilities", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Facilities", "Address");
        }
    }
}
