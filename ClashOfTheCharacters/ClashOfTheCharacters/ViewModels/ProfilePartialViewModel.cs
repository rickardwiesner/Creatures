using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class ProfilePartialViewModel
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string ImageUrl { get; set; }

        public int LadderPoints { get; set; }

        public int Level { get; set; }

        public int Rank { get; set; }

        public int Xp { get; set; }

        public int MaxXp { get; set; }

        public int Gold { get; set; }

        public int Stamina { get; set; }

        public int MaxStamina { get; set; }

        public int NextStaminaMinutes { get; set; }

        public ICollection<TeamMember> TeamMembers { get; set; }

        public ICollection<Battle> RecentBattles { get; set; }

        public ICollection<Battle> OngoingBattles { get; set; }
    }
}