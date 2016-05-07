using System;
using System.ComponentModel.DataAnnotations;

namespace ClashOfTheCharacters.Models
{
    public class UserCreature
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public int Xp { get; set; }

        public int Slot { get; set; }

        public int Battles { get; set; }

        public int Kills { get; set; }

        public int Deaths { get; set; }

        public bool InSquad { get; set; }

        public bool InAuction { get; set; }

        [Required]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int CreatureId { get; set; }
        public virtual Creature Creature { get; set; }

        public int Damage { get { return Convert.ToInt32(Level * Creature.AttackMultiplier + Creature.BaseAttack); } }

        public int Defense { get { return Convert.ToInt32(Level * Creature.DefenseMultiplier + Creature.BaseDefense); } }

        public int Hp { get { return Convert.ToInt32(Level * Creature.HpMultiplier + Creature.BaseHp); } }

        public int Worth { get { return Convert.ToInt32(Creature.Price / 2); } }

        public int MaxXp { get { return 50 + Level / 2 * 6 * Level; } }
    }
}