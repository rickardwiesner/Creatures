using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Competitor
    {
        public int Id { get; set; }

        public bool Winner { get; set; }

        public bool Challenger { get; set; }

        public int GoldEarned { get; set; }

        public int RankingPointsEarned { get; set; }

        public int XpEarned { get; set; }

        public int TotalHp { get { return BattleCharacters.Sum(bc => bc.Hp); } }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int BattleId { get; set; }
        public virtual Battle Battle { get; set; }

        public virtual ICollection<BattleCreature> BattleCharacters { get; set; }
    }
}