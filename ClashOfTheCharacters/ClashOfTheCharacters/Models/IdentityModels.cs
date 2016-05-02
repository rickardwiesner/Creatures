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

        public int Gold { get; set; }

        public int Xp { get; set; }

        public int MaxXp { get { return 50 + Level / 2 * 6 * Level; } }

        public int Level { get; set; }

        public int LadderPoints { get; set; }

        public bool Admin { get; set; }

        public DateTimeOffset LastActive { get; set; }

        public DateTimeOffset LastStaminaTime { get; set; }

        public bool IsOnline { get { return (DateTimeOffset.Now - LastActive).TotalMinutes < 30; } }

        public virtual ICollection<TeamMember> TeamMembers { get; set; }

        public virtual ICollection<UnlockedLand> UnlockedLands { get; set; }

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

        public DbSet<Character> Characters { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Challenge> Challenges { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<Attack> Attacks { get; set; }
        //public DbSet<BattleReward> BattleRewards { get; set; }
        public DbSet<Competitor> Competitors { get; set; }
        public DbSet<BattleCharacter> BattleCharacters { get; set; }
        public DbSet<Land> Lands { get; set; }
        public DbSet<Travel> Travels { get; set; }
        public DbSet<UnlockedLand> UnlockedLands { get; set; }
        public DbSet<CurrentLand> CurrentLands { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<WildCreature> WildCreatures { get; set; }
    }
}