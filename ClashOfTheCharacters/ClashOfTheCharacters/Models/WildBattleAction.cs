using ClashOfTheCharacters.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class WildBattleAction
    {
        public int Id { get; set; }

        public int? Damage { get; set; }

        public bool FinishingBlow { get { return Defender.Hp == 0 ? true : false; } }

        public Effect Effect { get; set; }

        public int WildBattleId { get; set; }
        public virtual WildBattle WildBattle { get; set; }

        public int? AttackerId { get; set; }
        public virtual WildBattleCreature Attacker { get; set; }

        public int? DefenderId { get; set; }
        public virtual WildBattleCreature Defender { get; set; }

        public bool CaptureAttempt { get; set; }

        public bool? CaptureSuccess { get; set; }
    }
}