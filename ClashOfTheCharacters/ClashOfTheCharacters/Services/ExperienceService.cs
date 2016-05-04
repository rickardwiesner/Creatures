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
            var winner = challengerCharacterWon ? db.BattleCharacters.Find(challengerCharacterId) : db.BattleCharacters.Find(receiverCharacterId);
            var loser = challengerCharacterWon ? db.BattleCharacters.Find(receiverCharacterId) : db.BattleCharacters.Find(challengerCharacterId);

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

            db.SaveChanges();

            AddXp(winner.Competitor.User.TeamMembers.First(tm => tm.CharacterId == winner.CharacterId).Id, winnerXp);
            AddXp(loser.Competitor.User.TeamMembers.First(tm => tm.CharacterId == winner.CharacterId).Id, loserXp);
        }

        public void AddXp(int teamMemberId, int xp)
        {
            var teamMember = db.TeamMembers.Find(teamMemberId);

            if (teamMember.Xp + xp >= teamMember.MaxXp)
            {
                int remaingXp = teamMember.MaxXp - teamMember.Xp;

                teamMember.Level++;
                teamMember.Xp = xp - remaingXp;
            }

            else
            {
                teamMember.Xp += xp;
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