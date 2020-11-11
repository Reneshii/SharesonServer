namespace SharesonServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Account : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountModels",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Email = c.String(),
                        Name = c.String(),
                        Surname = c.String(),
                        Login = c.String(),
                        Password = c.String(),
                        LoggedIn = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AccountModels");
        }
    }
}
