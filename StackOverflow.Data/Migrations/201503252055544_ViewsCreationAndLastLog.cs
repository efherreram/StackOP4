namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ViewsCreationAndLastLog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Accounts", "ViewsToProfile", c => c.Int(nullable: false));
            AddColumn("dbo.Accounts", "CreationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Accounts", "LastLogDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Accounts", "LastLogDate");
            DropColumn("dbo.Accounts", "CreationDate");
            DropColumn("dbo.Accounts", "ViewsToProfile");
        }
    }
}
