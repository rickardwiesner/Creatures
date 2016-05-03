using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class WildBattleCreature
    {
        public int Id { get; set; }

        public int? TeamMemberId { get; set; }
        public virtual TeamMember TeamMember { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }

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

        public int Damage { get { return Convert.ToInt32(Level * TeamMember.Character.AttackMultiplier + TeamMember.Character.BaseAttack); } }

        public int Defense { get { return Convert.ToInt32(Level * TeamMember.Character.DefenseMultiplier + TeamMember.Character.BaseDefense); } }
    }
}