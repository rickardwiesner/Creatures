using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class WildCreature
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }

        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }

        public int Hp { get; set; }

        public int MaxHp { get; set; }

        public int Slot { get; set; }

        public bool Alive { get { return Hp > 0; } }

        public int Damage { get { return Convert.ToInt32(Level * Character.AttackMultiplier + Character.BaseAttack); } }

        public int Defense { get { return Convert.ToInt32(Level * Character.DefenseMultiplier + Character.BaseDefense); } }
    }
}