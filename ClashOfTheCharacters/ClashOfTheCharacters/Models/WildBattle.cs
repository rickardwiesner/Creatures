using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class WildBattle
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public int StageId { get; set; }
        public virtual Stage Stage { get; set; }

        public bool Won { get; set; }

        public bool Finished { get; set; }

        public virtual ICollection<WildBattleCreature> WildBattleCreatures { get; set; }
        public virtual ICollection<WildBattleAction> WildBattleActions { get; set; }
    }
}