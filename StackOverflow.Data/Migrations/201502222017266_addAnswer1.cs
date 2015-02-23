namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAnswer1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answers", "QuestionReference_Id", c => c.Guid());
            CreateIndex("dbo.Answers", "QuestionReference_Id");
            AddForeignKey("dbo.Answers", "QuestionReference_Id", "dbo.Questions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Answers", "QuestionReference_Id", "dbo.Questions");
            DropIndex("dbo.Answers", new[] { "QuestionReference_Id" });
            DropColumn("dbo.Answers", "QuestionReference_Id");
        }
    }
}
