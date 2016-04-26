using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.ViewModels
{
    public class UserViewModel
    {
        public int TotalBattles { get { return WonBattles + LostBattles; } }

        public int WonBattles { get; set; }

        public int BattlesWonPercentage { get { return Convert.ToInt32((float)WonBattles / (float)TotalBattles * 100); } }

        public int LostBattles { get; set; }

        //100 - BattlesWonPercentage
        public int BattlesLostPercentage { get { return Convert.ToInt32((float)LostBattles / (float)TotalBattles * 100); } }

        public int TotalGoldEarned { get; set; }

        public TeamMember TeamMember { get; set; }


    }
}