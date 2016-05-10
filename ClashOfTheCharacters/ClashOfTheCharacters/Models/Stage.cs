using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Models
{
    public class Stage
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public int PositionX { get; set; }

        public int PositionY { get; set; }

        public int GoldReward { get; set; }

        public int XpReward { get; set; }

        public int NumberOfCreatures { get; set; }

        public int LandId { get; set; }

        public virtual Land Land { get; set; }
    }
}