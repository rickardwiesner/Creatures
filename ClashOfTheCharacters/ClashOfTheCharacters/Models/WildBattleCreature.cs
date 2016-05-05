using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class WildBattleCreature
    {
        public int Id { get; set; }

        public int CreatureId { get; set; }
        public virtual Creature Creature { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int WildBattleId { get; set; }
        public virtual WildBattle WildBattle { get; set; }

        public int Level { get; set; }

        public int Hp { get; set; }

        public int MaxHp { get; set; }

        public int XpEarned { get; set; }

        public int Slot { get; set; }

        public bool Alive { get { return Hp > 0; } }

        public int Damage { get { return Convert.ToInt32(Level * Creature.AttackMultiplier + Creature.BaseAttack); } }

        public int Defense { get { return Convert.ToInt32(Level * Creature.DefenseMultiplier + Creature.BaseDefense); } }
    }
}