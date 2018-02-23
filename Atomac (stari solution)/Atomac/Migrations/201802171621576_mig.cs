namespace Atomac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Artifacts", "OwnerId", "dbo.AspNetUsers");
            DropIndex("dbo.Artifacts", new[] { "OwnerId" });
            CreateTable(
                "dbo.Stuffs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Activity = c.Boolean(nullable: false),
                        ArtifactId = c.Int(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Artifacts", t => t.ArtifactId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.ArtifactId)
                .Index(t => t.OwnerId);
            
            AddColumn("dbo.Artifacts", "Prize", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "StatusT1", c => c.Int(nullable: false));
            AddColumn("dbo.Games", "StatusT2", c => c.Int(nullable: false));
            AddColumn("dbo.Moves", "Color", c => c.String());
            AddColumn("dbo.Moves", "Captured", c => c.String());
            AlterColumn("dbo.Moves", "State", c => c.String());
            DropColumn("dbo.Artifacts", "Activity");
            DropColumn("dbo.Artifacts", "OwnerId");
            DropColumn("dbo.Games", "Points");
            DropColumn("dbo.Moves", "White");
            DropColumn("dbo.Moves", "Black");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Moves", "Black", c => c.String(nullable: false));
            AddColumn("dbo.Moves", "White", c => c.String(nullable: false));
            AddColumn("dbo.Games", "Points", c => c.Int(nullable: false));
            AddColumn("dbo.Artifacts", "OwnerId", c => c.String(maxLength: 128));
            AddColumn("dbo.Artifacts", "Activity", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.Stuffs", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Stuffs", "ArtifactId", "dbo.Artifacts");
            DropIndex("dbo.Stuffs", new[] { "OwnerId" });
            DropIndex("dbo.Stuffs", new[] { "ArtifactId" });
            AlterColumn("dbo.Moves", "State", c => c.String(nullable: false));
            DropColumn("dbo.Moves", "Captured");
            DropColumn("dbo.Moves", "Color");
            DropColumn("dbo.Games", "StatusT2");
            DropColumn("dbo.Games", "StatusT1");
            DropColumn("dbo.Artifacts", "Prize");
            DropTable("dbo.Stuffs");
            CreateIndex("dbo.Artifacts", "OwnerId");
            AddForeignKey("dbo.Artifacts", "OwnerId", "dbo.AspNetUsers", "Id");
        }
    }
}
