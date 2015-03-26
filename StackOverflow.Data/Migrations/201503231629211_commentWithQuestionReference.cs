namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commentWithQuestionReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comments", "QuestionReference", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comments", "QuestionReference");
        }
    }
}
