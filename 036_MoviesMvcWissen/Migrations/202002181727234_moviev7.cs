namespace _036_MoviesMvcWissen.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class moviev7 : DbMigration
    {
        public override void Up()
        {
            string sql = "";
            sql = "drop view if exists dbo.vwUsers";
            Sql(sql);
            sql = "create view vwUsers as select ISNULL(ROW_NUMBER() over(order by u.Id), 0) as Id, u.Id as UserId,UserName, [Password] ,Active,RoleId,r.[Name] from Users u inner join Roles r on u.RoleId=r.Id";

            Sql(sql);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.vwUsers");
        }
    }
}
