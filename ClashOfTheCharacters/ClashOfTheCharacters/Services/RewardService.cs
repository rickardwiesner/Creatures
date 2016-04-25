using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class RewardService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void DistributeReward(int battleId)
        {
            var battle = db.Battles.Find(battleId);

            var winner = battle.Competitors.First(c => c.Winner);
            var loser = battle.Competitors.First(c => !c.Winner);

            int rankDifference = winner.User.Rank - loser.User.Rank;

            int winnerRankingPoints = 3;
            int winnerGold = 6;

            int loserRankingPoints = -3;
            int loserGold = 6;

            if (rankDifference < -300)
            {
                winnerRankingPoints = 15;
                winnerGold = 11;

                loserRankingPoints = -12;
                loserGold = 3;
            }

            else if (rankDifference < -150)
            {
                winnerRankingPoints = 12;
                winnerGold = 10;

                loserRankingPoints = -9;
                loserGold = 4;
            }

            else if (rankDifference >= -150 && rankDifference <= 150)
            {
                winnerRankingPoints = 9;
                winnerGold = 8;

                loserRankingPoints = -6;
                loserGold = 5;
            }

            else if (rankDifference < 300)
            {
                winnerRankingPoints = 6;
            }


            //Kolla så att ranken inte blir < 0 (0 är minimum)
            //...

            winner.RankingPointsEarned = winnerRankingPoints;
            winner.User.Rank += winnerRankingPoints;

            winner.GoldEarned = winnerGold;
            winner.User.Gold += winnerGold;

            loser.RankingPointsEarned = loserRankingPoints;
            loser.User.Rank += loserRankingPoints;

            loser.GoldEarned = loserGold;
            loser.User.Gold += loserGold;

            db.SaveChanges();
        }
    }
}