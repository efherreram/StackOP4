namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addVotes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Votes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AccountReference = c.Guid(nullable: false),
                        ReferenceToQuestionOrAnswer = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Votes");
        }
    }
}
