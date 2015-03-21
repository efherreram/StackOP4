namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isConfirmed : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "IsVerified", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "IsVerified");
        }
    }
}
