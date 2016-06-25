namespace CryptoMVC.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFinishedForExamAssginment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ExamAssignments", "Finished", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ExamAssignments", "Finished");
        }
    }
}
