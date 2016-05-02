namespace ClashOfTheCharacters.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Attacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Damage = c.Int(nullable: false),
                        DefenderHpRemaining = c.Int(nullable: false),
                        AttackerHpRemaining = c.Int(nullable: false),
                        Effect = c.Int(nullable: false),
                        BattleId = c.Int(nullable: false),
                        AttackerId = c.Int(),
                        DefenderId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Battles", t => t.BattleId, cascadeDelete: true)
                .ForeignKey("dbo.BattleCharacters", t => t.AttackerId)
                .ForeignKey("dbo.BattleCharacters", t => t.DefenderId)
                .Index(t => t.BattleId)
                .Index(t => t.AttackerId)
                .Index(t => t.DefenderId);
            
            CreateTable(
                "dbo.BattleCharacters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TeamMemberId = c.Int(nullable: false),
                        CompetitorId = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Hp = c.Int(nullable: false),
                        MaxHp = c.Int(nullable: false),
                        XpEarned = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TeamMembers", t => t.TeamMemberId, cascadeDelete: true)
                .ForeignKey("dbo.Competitors", t => t.CompetitorId, cascadeDelete: true)
                .Index(t => t.TeamMemberId)
                .Index(t => t.CompetitorId);
            
            CreateTable(
                "dbo.Competitors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Winner = c.Boolean(nullable: false),
                        Challenger = c.Boolean(nullable: false),
                        GoldEarned = c.Int(nullable: false),
                        RankingPointsEarned = c.Int(nullable: false),
                        XpEarned = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        BattleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Battles", t => t.BattleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.BattleId);
            
            CreateTable(
                "dbo.Battles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        Calculated = c.Boolean(nullable: false),
                        ChallengeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Challenges", t => t.ChallengeId, cascadeDelete: true)
                .Index(t => t.ChallengeId);
            
            CreateTable(
                "dbo.Challenges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Accepted = c.Boolean(nullable: false),
                        ChallengerId = c.String(maxLength: 128),
                        ReceiverId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ChallengerId)
                .ForeignKey("dbo.AspNetUsers", t => t.ReceiverId)
                .Index(t => t.ChallengerId)
                .Index(t => t.ReceiverId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ImageUrl = c.String(),
                        Stamina = c.Int(nullable: false),
                        MaxStamina = c.Int(nullable: false),
                        Gold = c.Int(nullable: false),
                        Xp = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        LadderPoints = c.Int(nullable: false),
                        Admin = c.Boolean(nullable: false),
                        LastActive = c.DateTimeOffset(nullable: false, precision: 7),
                        LastStaminaTime = c.DateTimeOffset(nullable: false, precision: 7),
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
                "dbo.TeamMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Xp = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                        ApplicationUserId = c.String(nullable: false, maxLength: 128),
                        CharacterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Characters", t => t.CharacterId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUserId, cascadeDelete: true)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.CharacterId);
            
            CreateTable(
                "dbo.Characters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Element = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Price = c.Int(nullable: false),
                        BaseAttack = c.Int(nullable: false),
                        BaseDefense = c.Int(nullable: false),
                        BaseHp = c.Int(nullable: false),
                        AttackMultiplier = c.Single(nullable: false),
                        DefenseMultiplier = c.Single(nullable: false),
                        HpMultiplier = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
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
            DropForeignKey("dbo.Attacks", "DefenderId", "dbo.BattleCharacters");
            DropForeignKey("dbo.Attacks", "AttackerId", "dbo.BattleCharacters");
            DropForeignKey("dbo.Competitors", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BattleCharacters", "CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Competitors", "BattleId", "dbo.Battles");
            DropForeignKey("dbo.Battles", "ChallengeId", "dbo.Challenges");
            DropForeignKey("dbo.Challenges", "ReceiverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Challenges", "ChallengerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamMembers", "ApplicationUserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TeamMembers", "CharacterId", "dbo.Characters");
            DropForeignKey("dbo.BattleCharacters", "TeamMemberId", "dbo.TeamMembers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Attacks", "BattleId", "dbo.Battles");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.TeamMembers", new[] { "CharacterId" });
            DropIndex("dbo.TeamMembers", new[] { "ApplicationUserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Challenges", new[] { "ReceiverId" });
            DropIndex("dbo.Challenges", new[] { "ChallengerId" });
            DropIndex("dbo.Battles", new[] { "ChallengeId" });
            DropIndex("dbo.Competitors", new[] { "BattleId" });
            DropIndex("dbo.Competitors", new[] { "UserId" });
            DropIndex("dbo.BattleCharacters", new[] { "CompetitorId" });
            DropIndex("dbo.BattleCharacters", new[] { "TeamMemberId" });
            DropIndex("dbo.Attacks", new[] { "DefenderId" });
            DropIndex("dbo.Attacks", new[] { "AttackerId" });
            DropIndex("dbo.Attacks", new[] { "BattleId" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Characters");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Challenges");
            DropTable("dbo.Battles");
            DropTable("dbo.Competitors");
            DropTable("dbo.BattleCharacters");
            DropTable("dbo.Attacks");
        }
    }
}
