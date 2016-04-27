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

            int rankDifference = winner.User.LadderPoints - loser.User.LadderPoints;

            int winnerRankingPoints = 0;
            int winnerGold = 0;
            int winnerXp = 0;

            int loserRankingPoints = 0;
            int loserGold = 0;
            int loserXp = 0;

            if (rankDifference < -300)
            {
                winnerRankingPoints = 15;
                winnerGold = 14;
                winnerXp = 18;

                loserRankingPoints = -12;
                loserGold = 3;
                loserXp = 4;
            }

            else if (rankDifference < -200)
            {
                winnerRankingPoints = 12;
                winnerGold = 12;
                winnerXp = 16;

                loserRankingPoints = -9;
                loserGold = 4;
                loserXp = 5;
            }

            else if (rankDifference >= -100 && rankDifference <= 100)
            {
                winnerRankingPoints = 9;
                winnerGold = 10;
                winnerXp = 14;

                loserRankingPoints = -6;
                loserGold = 6;
                loserXp = 7;
            }

            else if (rankDifference < 200)
            {
                winnerRankingPoints = 6;
                winnerGold = 8;
                winnerXp = 12;

                loserRankingPoints = -4;
                loserGold = 7;
                loserXp = 8;
            }

            else if (rankDifference < 300)
            {
                winnerRankingPoints = 3;
                winnerGold = 7;
                winnerXp = 10;

                loserRankingPoints = -2;
                loserGold = 7;
                loserXp = 9;
            }

            else
            {
                winnerRankingPoints = 1;
                winnerGold = 6;
                winnerXp = 9;

                loserRankingPoints = -1;
                loserGold = 6;
                loserXp = 9;
            }

            winner.GoldEarned = winnerGold;
            winner.User.Gold += winnerGold;          

            winner.RankingPointsEarned = winnerRankingPoints;
            winner.User.LadderPoints += winnerRankingPoints;

            winner.XpEarned = winnerXp;

            loser.GoldEarned = loserGold;
            loser.User.Gold += loserGold;

            loser.XpEarned = loserXp;

            loser.RankingPointsEarned = loserRankingPoints;

            if (loser.User.LadderPoints + loserRankingPoints >= 0)
            {
                loser.User.LadderPoints += loserRankingPoints;
            }

            else
            {
                loser.User.LadderPoints = 0;
            }

            var experienceService = new ExperienceService();

            experienceService.AddXp(winner.UserId, winnerXp);
            experienceService.AddXp(loser.UserId, loserXp);

            db.SaveChanges();
        }
    }
}