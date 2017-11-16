namespace Atomac.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AFigures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Style = c.String(nullable: false),
                        Activity = c.Boolean(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false, maxLength: 64),
                        LastName = c.String(nullable: false, maxLength: 64),
                        NickName = c.String(nullable: false, maxLength: 64),
                        Points = c.Int(nullable: false),
                        Tokens = c.Int(nullable: false),
                        Picture = c.String(),
                        Title = c.Int(nullable: false),
                        Wins = c.Int(nullable: false),
                        Draws = c.Int(nullable: false),
                        Losses = c.Int(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Teams",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 64),
                        Activity = c.Boolean(nullable: false),
                        Points = c.Int(nullable: false),
                        CapitenId = c.String(maxLength: 128),
                        TeamMemberId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.CapitenId)
                .ForeignKey("dbo.AspNetUsers", t => t.TeamMemberId)
                .Index(t => t.CapitenId)
                .Index(t => t.TeamMemberId);
            
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Points = c.Int(nullable: false),
                        Tokens = c.Int(nullable: false),
                        Duration = c.Int(nullable: false),
                        Team1Id = c.Int(nullable: false),
                        Team2Id = c.Int(nullable: false),
                        RulesId = c.Int(nullable: false),
                        Team_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rules", t => t.RulesId, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team1Id, cascadeDelete: true)
                .ForeignKey("dbo.Teams", t => t.Team_Id)
                .Index(t => t.Team1Id)
                .Index(t => t.RulesId)
                .Index(t => t.Team_Id);
            
            CreateTable(
                "dbo.Moves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Board = c.Int(nullable: false),
                        State = c.String(nullable: false),
                        White = c.String(nullable: false),
                        Black = c.String(nullable: false),
                        GameId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Games", t => t.GameId, cascadeDelete: true)
                .Index(t => t.GameId);
            
            CreateTable(
                "dbo.Rules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DroppedCheck = c.Boolean(nullable: false),
                        DroppedCheckMate = c.Boolean(nullable: false),
                        DroppedPawnOnFirstLine = c.Boolean(nullable: false),
                        DroppedPawnOnLastLine = c.Boolean(nullable: false),
                        DroppedFigureOnLastLine = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false, maxLength: 256),
                        LinkTag = c.Boolean(nullable: false),
                        Time = c.DateTime(nullable: false),
                        SenderId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.SenderId)
                .Index(t => t.SenderId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.ATables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Style = c.String(nullable: false),
                        Activity = c.Boolean(nullable: false),
                        OwnerId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.OwnerId)
                .Index(t => t.OwnerId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Teams", "TeamMemberId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ATables", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Messages", "SenderId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AFigures", "OwnerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Teams", "CapitenId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Games", "Team_Id", "dbo.Teams");
            DropForeignKey("dbo.Games", "Team1Id", "dbo.Teams");
            DropForeignKey("dbo.Games", "RulesId", "dbo.Rules");
            DropForeignKey("dbo.Moves", "GameId", "dbo.Games");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.ATables", new[] { "OwnerId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.Messages", new[] { "SenderId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.Moves", new[] { "GameId" });
            DropIndex("dbo.Games", new[] { "Team_Id" });
            DropIndex("dbo.Games", new[] { "RulesId" });
            DropIndex("dbo.Games", new[] { "Team1Id" });
            DropIndex("dbo.Teams", new[] { "TeamMemberId" });
            DropIndex("dbo.Teams", new[] { "CapitenId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AFigures", new[] { "OwnerId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.ATables");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.Messages");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.Rules");
            DropTable("dbo.Moves");
            DropTable("dbo.Games");
            DropTable("dbo.Teams");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AFigures");
        }
    }
}
