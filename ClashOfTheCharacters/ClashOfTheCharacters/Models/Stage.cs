using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Stage
    {
        public int Id { get; set; }

        public int Index { get; set; }

        public int GoldReward { get; set; }

        public int XpReward { get; set; }

        public int LandId { get; set; }
        public virtual Land Land { get; set; }

        //Tvek
        //public virtual ICollection<WildCreature> WildCreatures { get; set; }
    }
}