using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System;

namespace ClashOfTheCharacters.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string ImageUrl { get; set; }

        public int Stamina { get; set; }

        public int MaxStamina { get; set; }

        public int RainbowGems { get; set; }

        public int Gold { get; set; }

        public int Xp { get; set; }

        public int MaxXp { get { return 50 + Level / 2 * 6 * Level; } }

        public int Level { get; set; }

        public int LadderPoints { get; set; }

        public bool Admin { get; set; }

        public int Battles { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public DateTimeOffset LastActive { get; set; }

        public DateTimeOffset LastStaminaTime { get; set; }

        public bool IsOnline { get { return (DateTimeOffset.Now - LastActive).TotalMinutes < 30; } }

        public virtual ICollection<UserCreature> UserCreatures { get; set; }

        public virtual ICollection<UnlockedLand> UnlockedLands { get; set; }

        public virtual ICollection<ClearedLand> ClearedLands { get; set; }

        public virtual ICollection<UserItem> UserItems { get; set; }

        public virtual ICollection<AuctionTarget> AuctionTargets { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        //För databas hos Binero
        //public ApplicationDbContext()
        //    : base()
        //{

        //}

        //För lokal databas
        public ApplicationDbContext()
                   : base("EarthOfCreatures", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<TeamCreature>()
        //        .HasRequired(m => m.User)
        //        .WithMany(uc => uc.UserCreatures)
        //        .WillCascadeOnDelete(true);

        //    modelBuilder.Entity<ApplicationUser>()
        //        .HasMany(m => m.TeamCreatures)
        //        .WithRequired(uc => uc.User)
        //        .WillCascadeOnDelete(true);

        //    base.OnModelCreating(modelBuilder);
        //}


        public DbSet<Creature> Creatures { get; set; }
        public DbSet<UserCreature> UserCreatures { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Attack> Attacks { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<BattleCreature> BattleCreatures { get; set; }
        public DbSet<Land> Lands { get; set; }
        public DbSet<Travel> Travels { get; set; }
        public DbSet<UnlockedLand> UnlockedLands { get; set; }
        public DbSet<CurrentLand> CurrentLands { get; set; }
        public DbSet<ClearedLand> ClearedLands { get; set; }
        public DbSet<WildBattle> WildBattles { get; set; }
        public DbSet<WildBattleCreature> WildBattleCreatures { get; set; }
        public DbSet<WildBattleAction> WildBattleActions { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<AuctionItem> AuctionItems { get; set; }
        public DbSet<AuctionCreature> AuctionCreatures { get; set; }
        public DbSet<AuctionTarget> AuctionTargets { get; set; }
    }
}