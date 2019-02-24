namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class splitplayerfullnametofirstlastfields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Players", "FirstName", c => c.String());
            AddColumn("dbo.Players", "LastName", c => c.String());
            DropColumn("dbo.Players", "FullName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Players", "FullName", c => c.String());
            DropColumn("dbo.Players", "LastName");
            DropColumn("dbo.Players", "FirstName");
        }
    }
}
