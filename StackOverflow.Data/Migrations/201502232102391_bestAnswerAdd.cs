namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bestAnswerAdd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "IsBestAnswer", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answers", "IsBestAnswer");
        }
    }
}
