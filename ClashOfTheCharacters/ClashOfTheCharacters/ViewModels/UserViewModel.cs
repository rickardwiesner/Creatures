using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class UserViewModel
    {
        public string Username { get; set; }

        public int TotalBattles { get { return WonBattles + LostBattles; } }

        public int WonBattles { get; set; }

        public int WinPercentage { get { return TotalBattles == 0 ? 0 : Convert.ToInt32((float)WonBattles / TotalBattles * 100); } }

        public int LostBattles { get; set; }

        public int LosePercentage { get { return TotalBattles == 0 ? 0 : 100 - WinPercentage; } }

        public int TotalGoldEarned { get; set; }

        public TeamMember MostUsedCharacter { get; set; }

        public TeamMember MostValuedCharacter { get; set; }

    }
}