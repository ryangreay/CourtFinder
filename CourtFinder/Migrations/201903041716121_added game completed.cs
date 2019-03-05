namespace CourtFinder.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedgamecompleted : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Games", "GameCompleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Games", "GameCompleted");
        }
    }
}
