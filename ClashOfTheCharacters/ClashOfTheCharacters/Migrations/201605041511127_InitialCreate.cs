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
                .ForeignKey("dbo.BattleCreatures", t => t.AttackerId)
                .ForeignKey("dbo.BattleCreatures", t => t.DefenderId)
                .Index(t => t.BattleId)
                .Index(t => t.AttackerId)
                .Index(t => t.DefenderId);
            
            CreateTable(
                "dbo.BattleCreatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatureId = c.Int(nullable: false),
                        CompetitorId = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Hp = c.Int(nullable: false),
                        MaxHp = c.Int(nullable: false),
                        XpEarned = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Competitors", t => t.CompetitorId, cascadeDelete: true)
                .ForeignKey("dbo.Creatures", t => t.CreatureId, cascadeDelete: true)
                .Index(t => t.CreatureId)
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
                "dbo.ClearedLands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        LandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lands", t => t.LandId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.LandId);
            
            CreateTable(
                "dbo.Lands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Element = c.Int(nullable: false),
                        Cost = c.Int(nullable: false),
                        Hours = c.Int(nullable: false),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Stages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Index = c.Int(nullable: false),
                        GoldReward = c.Int(nullable: false),
                        XpReward = c.Int(nullable: false),
                        LandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lands", t => t.LandId, cascadeDelete: true)
                .Index(t => t.LandId);
            
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
                "dbo.UnlockedLands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        LandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lands", t => t.LandId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.LandId);
            
            CreateTable(
                "dbo.UserCreatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Level = c.Int(nullable: false),
                        Xp = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                        Battles = c.Int(nullable: false),
                        Kills = c.Int(nullable: false),
                        Deaths = c.Int(nullable: false),
                        InSquad = c.Boolean(nullable: false),
                        UserId = c.String(nullable: false, maxLength: 128),
                        CreatureId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Creatures", t => t.CreatureId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.CreatureId);
            
            CreateTable(
                "dbo.Creatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Element = c.Int(nullable: false),
                        ImageUrl = c.String(),
                        Price = c.Int(nullable: false),
                        Rarity = c.Int(nullable: false),
                        BaseAttack = c.Int(nullable: false),
                        BaseDefense = c.Int(nullable: false),
                        BaseHp = c.Int(nullable: false),
                        AttackMultiplier = c.Single(nullable: false),
                        DefenseMultiplier = c.Single(nullable: false),
                        HpMultiplier = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CurrentLands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WildCreatureStartLevel = c.Int(nullable: false),
                        CurrentStageIndex = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        LandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lands", t => t.LandId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.LandId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.Travels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ArrivalTime = c.DateTimeOffset(nullable: false, precision: 7),
                        UserId = c.String(maxLength: 128),
                        LandId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Lands", t => t.LandId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.LandId);
            
            CreateTable(
                "dbo.WildBattleActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Damage = c.Int(),
                        Effect = c.Int(nullable: false),
                        WildBattleId = c.Int(nullable: false),
                        AttackerId = c.Int(),
                        DefenderId = c.Int(),
                        CaptureAttempt = c.Boolean(nullable: false),
                        CaptureSuccess = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WildBattles", t => t.WildBattleId, cascadeDelete: true)
                .ForeignKey("dbo.WildBattleCreatures", t => t.AttackerId)
                .ForeignKey("dbo.WildBattleCreatures", t => t.DefenderId)
                .Index(t => t.WildBattleId)
                .Index(t => t.AttackerId)
                .Index(t => t.DefenderId);
            
            CreateTable(
                "dbo.WildBattleCreatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CreatureId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        WildBattleId = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Hp = c.Int(nullable: false),
                        MaxHp = c.Int(nullable: false),
                        XpEarned = c.Int(nullable: false),
                        Slot = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Creatures", t => t.CreatureId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .ForeignKey("dbo.WildBattles", t => t.WildBattleId, cascadeDelete: true)
                .Index(t => t.CreatureId)
                .Index(t => t.UserId)
                .Index(t => t.WildBattleId);
            
            CreateTable(
                "dbo.WildBattles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        StageId = c.Int(nullable: false),
                        Won = c.Boolean(nullable: false),
                        Finished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Stages", t => t.StageId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.StageId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WildBattleActions", "DefenderId", "dbo.WildBattleCreatures");
            DropForeignKey("dbo.WildBattleActions", "AttackerId", "dbo.WildBattleCreatures");
            DropForeignKey("dbo.WildBattleCreatures", "WildBattleId", "dbo.WildBattles");
            DropForeignKey("dbo.WildBattleActions", "WildBattleId", "dbo.WildBattles");
            DropForeignKey("dbo.WildBattles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WildBattles", "StageId", "dbo.Stages");
            DropForeignKey("dbo.WildBattleCreatures", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.WildBattleCreatures", "CreatureId", "dbo.Creatures");
            DropForeignKey("dbo.Travels", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Travels", "LandId", "dbo.Lands");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CurrentLands", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CurrentLands", "LandId", "dbo.Lands");
            DropForeignKey("dbo.Attacks", "DefenderId", "dbo.BattleCreatures");
            DropForeignKey("dbo.Attacks", "AttackerId", "dbo.BattleCreatures");
            DropForeignKey("dbo.BattleCreatures", "CreatureId", "dbo.Creatures");
            DropForeignKey("dbo.Competitors", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.BattleCreatures", "CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Competitors", "BattleId", "dbo.Battles");
            DropForeignKey("dbo.Battles", "ChallengeId", "dbo.Challenges");
            DropForeignKey("dbo.Challenges", "ReceiverId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Challenges", "ChallengerId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCreatures", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UserCreatures", "CreatureId", "dbo.Creatures");
            DropForeignKey("dbo.UnlockedLands", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.UnlockedLands", "LandId", "dbo.Lands");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClearedLands", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ClearedLands", "LandId", "dbo.Lands");
            DropForeignKey("dbo.Stages", "LandId", "dbo.Lands");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Attacks", "BattleId", "dbo.Battles");
            DropIndex("dbo.WildBattles", new[] { "StageId" });
            DropIndex("dbo.WildBattles", new[] { "UserId" });
            DropIndex("dbo.WildBattleCreatures", new[] { "WildBattleId" });
            DropIndex("dbo.WildBattleCreatures", new[] { "UserId" });
            DropIndex("dbo.WildBattleCreatures", new[] { "CreatureId" });
            DropIndex("dbo.WildBattleActions", new[] { "DefenderId" });
            DropIndex("dbo.WildBattleActions", new[] { "AttackerId" });
            DropIndex("dbo.WildBattleActions", new[] { "WildBattleId" });
            DropIndex("dbo.Travels", new[] { "LandId" });
            DropIndex("dbo.Travels", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.CurrentLands", new[] { "LandId" });
            DropIndex("dbo.CurrentLands", new[] { "UserId" });
            DropIndex("dbo.UserCreatures", new[] { "CreatureId" });
            DropIndex("dbo.UserCreatures", new[] { "UserId" });
            DropIndex("dbo.UnlockedLands", new[] { "LandId" });
            DropIndex("dbo.UnlockedLands", new[] { "UserId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.Stages", new[] { "LandId" });
            DropIndex("dbo.ClearedLands", new[] { "LandId" });
            DropIndex("dbo.ClearedLands", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Challenges", new[] { "ReceiverId" });
            DropIndex("dbo.Challenges", new[] { "ChallengerId" });
            DropIndex("dbo.Battles", new[] { "ChallengeId" });
            DropIndex("dbo.Competitors", new[] { "BattleId" });
            DropIndex("dbo.Competitors", new[] { "UserId" });
            DropIndex("dbo.BattleCreatures", new[] { "CompetitorId" });
            DropIndex("dbo.BattleCreatures", new[] { "CreatureId" });
            DropIndex("dbo.Attacks", new[] { "DefenderId" });
            DropIndex("dbo.Attacks", new[] { "AttackerId" });
            DropIndex("dbo.Attacks", new[] { "BattleId" });
            DropTable("dbo.WildBattles");
            DropTable("dbo.WildBattleCreatures");
            DropTable("dbo.WildBattleActions");
            DropTable("dbo.Travels");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.CurrentLands");
            DropTable("dbo.Creatures");
            DropTable("dbo.UserCreatures");
            DropTable("dbo.UnlockedLands");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.Stages");
            DropTable("dbo.Lands");
            DropTable("dbo.ClearedLands");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Challenges");
            DropTable("dbo.Battles");
            DropTable("dbo.Competitors");
            DropTable("dbo.BattleCreatures");
            DropTable("dbo.Attacks");
        }
    }
}
