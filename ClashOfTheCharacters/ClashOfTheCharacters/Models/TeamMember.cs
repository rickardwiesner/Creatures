using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class TeamMember
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }

        public int Damage
        {
            get
            {
                return Convert.ToInt32(Level * Character.AttackMultiplier + Character.BaseAttack);
            }
        }

        public int Defense
        {
            get
            {
                return Convert.ToInt32(Level * Character.DefenseMultiplier + Character.BaseDefense);
            }
        }

        public int Hp
        {
            get
            {
                return Convert.ToInt32(Level * Character.HpMultiplier + Character.BaseHp);
            }
        }

        public int Worth
        {
            get
            {
                return Convert.ToInt32((Character.Price * 0.5) + (Level * 5));
            }
        }

        public int MaxXp
        {
            get
            {
                return 50 + Level / 2 * 6 * Level;
            }
        }
    }
}