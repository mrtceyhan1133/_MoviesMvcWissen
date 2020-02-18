namespace _036_MoviesMvcWissen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Moviesv04 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "Active");
        }
    }
}
