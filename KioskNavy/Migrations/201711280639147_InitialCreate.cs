namespace KioskNavy.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminSettingsModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BackImageUrl = c.String(),
                        FontSize = c.String(),
                        FontColor = c.String(),
                        Backcolor = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.LearningModels",
                c => new
                    {
                        TopicName = c.String(nullable: false, maxLength: 128),
                        SubjectName = c.String(nullable: false),
                        subsubject = c.String(nullable: false),
                        ID = c.Int(nullable: false, identity: true),
                        noofpage = c.String(),
                        pdfURL = c.String(),
                        pptURL = c.String(),
                        vidURL = c.String(),
                        imgURL = c.String(),
                        content = c.String(),
                    })
                .PrimaryKey(t => t.TopicName);
            
            CreateTable(
                "dbo.LevelNameModels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Level = c.String(),
                        SubjectName = c.String(maxLength: 128),
                        LevelDescription = c.String(),
                        certificateTreshhold = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuizNameModels", t => t.SubjectName)
                .Index(t => t.SubjectName);
            
            CreateTable(
                "dbo.QuizNameModels",
                c => new
                    {
                        SubjectName = c.String(nullable: false, maxLength: 128),
                        ID = c.Int(nullable: false, identity: true),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectName);
            
            CreateTable(
                "dbo.subSubjectmdels",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SubSubject = c.String(),
                        SubjectName = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.QuizNameModels", t => t.SubjectName)
                .Index(t => t.SubjectName);
            
            CreateTable(
                "dbo.QuizQuestionModels",
                c => new
                    {
                        QuestionNumber = c.Int(nullable: false),
                        QuizName = c.String(nullable: false, maxLength: 128),
                        Level = c.String(nullable: false, maxLength: 128),
                        SubSubject = c.String(nullable: false, maxLength: 128),
                        ID = c.Int(nullable: false, identity: true),
                        QuestionType = c.String(),
                        Question = c.String(nullable: false),
                        QuestionHighlighted = c.String(),
                        answer = c.String(nullable: false),
                        hint1 = c.String(),
                        choic1 = c.String(),
                        choic2 = c.String(),
                        choic3 = c.String(),
                        choic4 = c.String(),
                        choic5 = c.String(),
                        choic6 = c.String(),
                        URLchoic1 = c.String(),
                        URLchoic2 = c.String(),
                        URLchoic3 = c.String(),
                        URLchoic4 = c.String(),
                        URLchoic5 = c.String(),
                        URLchoic6 = c.String(),
                        IsImage = c.Boolean(nullable: false),
                        IsVideo = c.Boolean(nullable: false),
                        imageURL = c.String(),
                        vidURL = c.String(),
                        furthurInfo = c.String(),
                        furthurInfoimageURL = c.String(),
                        furthurInfoVidURL = c.String(),
                    })
                .PrimaryKey(t => new { t.QuestionNumber, t.QuizName, t.Level, t.SubSubject });
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.subSubjectmdels", "SubjectName", "dbo.QuizNameModels");
            DropForeignKey("dbo.LevelNameModels", "SubjectName", "dbo.QuizNameModels");
            DropIndex("dbo.subSubjectmdels", new[] { "SubjectName" });
            DropIndex("dbo.LevelNameModels", new[] { "SubjectName" });
            DropTable("dbo.QuizQuestionModels");
            DropTable("dbo.subSubjectmdels");
            DropTable("dbo.QuizNameModels");
            DropTable("dbo.LevelNameModels");
            DropTable("dbo.LearningModels");
            DropTable("dbo.AdminSettingsModels");
        }
    }
}
