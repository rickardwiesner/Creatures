using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class BattleCharacter
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }
        public virtual Character Character { get; set; }

        public int CompetitorId { get; set; }
        public virtual Competitor Competitor { get; set; }

        public int Level { get; set; }
        
        //Borde skickas till TeamMember
        public int Slot { get; set; }

        public int Hp { get; set; }

        public int MaxHp { get; set; }

        public bool Alive { get { return Hp > 0; } }

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
    }
}