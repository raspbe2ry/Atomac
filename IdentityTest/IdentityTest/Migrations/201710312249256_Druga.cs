namespace IdentityTest.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Druga : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Nestoes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        KlasaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Klasas", t => t.KlasaId, cascadeDelete: true)
                .Index(t => t.KlasaId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Nestoes", "KlasaId", "dbo.Klasas");
            DropIndex("dbo.Nestoes", new[] { "KlasaId" });
            DropTable("dbo.Nestoes");
        }
    }
}
