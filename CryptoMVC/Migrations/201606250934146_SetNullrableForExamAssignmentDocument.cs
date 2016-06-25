namespace CryptoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetNullrableForExamAssignmentDocument : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ExamAssignments", "DocumentId", "dbo.Documents");
            DropIndex("dbo.ExamAssignments", new[] { "DocumentId" });
            AlterColumn("dbo.ExamAssignments", "DocumentId", c => c.Int());
            CreateIndex("dbo.ExamAssignments", "DocumentId");
            AddForeignKey("dbo.ExamAssignments", "DocumentId", "dbo.Documents", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ExamAssignments", "DocumentId", "dbo.Documents");
            DropIndex("dbo.ExamAssignments", new[] { "DocumentId" });
            AlterColumn("dbo.ExamAssignments", "DocumentId", c => c.Int(nullable: false));
            CreateIndex("dbo.ExamAssignments", "DocumentId");
            AddForeignKey("dbo.ExamAssignments", "DocumentId", "dbo.Documents", "Id", cascadeDelete: true);
        }
    }
}
