namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commentWithAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "Owner_Id", c => c.Guid());
            CreateIndex("dbo.Comments", "Owner_Id");
            AddForeignKey("dbo.Comments", "Owner_Id", "dbo.Accounts", "Id");
            DropColumn("dbo.Comments", "OwnerId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Comments", "OwnerId", c => c.Guid(nullable: false));
            DropForeignKey("dbo.Comments", "Owner_Id", "dbo.Accounts");
            DropIndex("dbo.Comments", new[] { "Owner_Id" });
            DropColumn("dbo.Comments", "Owner_Id");
        }
    }
}
