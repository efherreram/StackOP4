namespace StackOverflow.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Accounts",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        IsVerified = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AnswerText = c.String(),
                        Votes = c.Int(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        IsBestAnswer = c.Boolean(nullable: false),
                        NumberOfViews = c.Int(nullable: false),
                        Owner_Id = c.Guid(),
                        QuestionReference_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Owner_Id)
                .ForeignKey("dbo.Questions", t => t.QuestionReference_Id)
                .Index(t => t.Owner_Id)
                .Index(t => t.QuestionReference_Id);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Votes = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        ModificationDate = c.DateTime(nullable: false),
                        NumberOfViews = c.Int(nullable: false),
                        Owner_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Accounts", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Description = c.String(),
                        ReferenceToQuestionOrAnswer = c.Guid(nullable: false),
                        OwnerId = c.Guid(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        Votes = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.Answers", "QuestionReference_Id", "dbo.Questions");
            DropForeignKey("dbo.Questions", "Owner_Id", "dbo.Accounts");
            DropForeignKey("dbo.Answers", "Owner_Id", "dbo.Accounts");
            DropIndex("dbo.Questions", new[] { "Owner_Id" });
            DropIndex("dbo.Answers", new[] { "QuestionReference_Id" });
            DropIndex("dbo.Answers", new[] { "Owner_Id" });
            DropTable("dbo.Votes");
            DropTable("dbo.Comments");
            DropTable("dbo.Questions");
            DropTable("dbo.Answers");
            DropTable("dbo.Accounts");
        }
    }
}
