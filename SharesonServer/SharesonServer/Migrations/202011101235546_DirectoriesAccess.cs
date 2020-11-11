namespace SharesonServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DirectoriesAccess : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DirectoriesAccessedInAccounts",
                c => new
                    {
                        ID = c.String(nullable: false, maxLength: 128),
                        Directories = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DirectoriesAccessedInAccounts");
        }
    }
}
