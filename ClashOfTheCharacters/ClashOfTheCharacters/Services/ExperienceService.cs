using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class ExperienceService
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public void CalculateXp(int challengerCharacterId, int receiverCharacterId, bool challengerCharacterWon)
        {
            var winner = challengerCharacterWon ? db.BattleCreatures.Find(challengerCharacterId) : db.BattleCreatures.Find(receiverCharacterId);
            var loser = challengerCharacterWon ? db.BattleCreatures.Find(receiverCharacterId) : db.BattleCreatures.Find(challengerCharacterId);

            int levelDifference = winner.Level - loser.Level;

            int winnerXp = 8;
            int loserXp = 4;

            if (levelDifference < -10)
            {
                winnerXp = 24;
            }

            else if (levelDifference < -5)
            {
                winnerXp = 22;
            }

            else if (levelDifference < -1)
            {
                winnerXp = 20;
                loserXp = 6;
            }

            else if (levelDifference < 2)
            {
                winnerXp = 18;
                loserXp = 8;
            }

            else if (levelDifference < 6)
            {
                winnerXp = 16;
                loserXp = 10;
            }

            else if (levelDifference < 10)
            {
                winnerXp = 14;
                loserXp = 12;
            }

            winner.XpEarned += winnerXp;
            loser.XpEarned += loserXp;

            var winnerUserCreature = winner.Competitor.User.UserCreatures.Where(uc => uc.InSquad).First(tm => tm.CreatureId == winner.CreatureId);
            winnerUserCreature.Battles++;
            winnerUserCreature.Kills++;

            var loserUserCreature = loser.Competitor.User.UserCreatures.Where(uc => uc.InSquad).First(tm => tm.CreatureId == loser.CreatureId);
            loserUserCreature.Battles++;
            loserUserCreature.Deaths++;

            db.SaveChanges();
            
            AddXp(winnerUserCreature.Id, winnerXp);
            AddXp(loserUserCreature.Id, loserXp);
        }

        public void CalculateWildXp(int wildBattleCreatureId, int opponentCreatureLevel, bool won)
        {
            var wildBattleCreature = db.WildBattleCreatures.Find(wildBattleCreatureId);
            var levelDifference = wildBattleCreature.Level - opponentCreatureLevel;
            var xp = 0;

            if (levelDifference < -10)
            {
                xp = won ? 24 : 4;
            }

            else if (levelDifference < -5)
            {
                xp = won ? 22 : 4;
            }

            else if (levelDifference < -1)
            {
                xp = won ? 20 : 6;
            }

            else if (levelDifference < 2)
            {
                xp = won ? 18 : 6;
            }

            else if (levelDifference < 6)
            {
                xp = won ? 16 : 8;
            }

            else if (levelDifference < 10)
            {
                xp = won ? 14 : 8;
            }

            else
            {
                xp = won ? 12 : 8;
            }

            var userCreature = wildBattleCreature.User.UserCreatures.Where(uc => uc.InSquad).First(uc => uc.CreatureId == wildBattleCreature.CreatureId);

            wildBattleCreature.XpEarned += xp;
            db.SaveChanges();

            AddXp(userCreature.Id, xp);
        }

        public void AddXp(int userCreatureId, int xp)
        {
            var userCreature = db.UserCreatures.Find(userCreatureId);

            if (userCreature.Xp + xp >= userCreature.MaxXp)
            {
                int remaingXp = userCreature.MaxXp - userCreature.Xp;

                userCreature.Level++;
                userCreature.Xp = xp - remaingXp;
            }

            else
            {
                userCreature.Xp += xp;
            }

            db.SaveChanges();
        }

        public void AddXp(string userId, int xp)
        {
            var user = db.Users.Find(userId);

            if (user.Xp + xp >= user.MaxXp)
            {
                int remaingXp = user.MaxXp - user.Xp;

                user.Level++;
                user.Xp = xp - remaingXp;
                //Kolla så att användaren inte går upp ytterligare en level på nåt sjukt sätt.
            }

            else
            {
                user.Xp += xp;
            }

            db.SaveChanges();
        }
    }
}