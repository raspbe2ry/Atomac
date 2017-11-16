namespace Atomac_Sah_.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cetvrta : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Teams", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Teams", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropIndex("dbo.Teams", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.Teams", new[] { "ApplicationUser_Id1" });
            DropColumn("dbo.Teams", "ApplicationUser_Id");
            DropColumn("dbo.Teams", "ApplicationUser_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Teams", "ApplicationUser_Id1", c => c.String(maxLength: 128));
            AddColumn("dbo.Teams", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Teams", "ApplicationUser_Id1");
            CreateIndex("dbo.Teams", "ApplicationUser_Id");
            AddForeignKey("dbo.Teams", "ApplicationUser_Id1", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Teams", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
