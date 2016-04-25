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

        public void CalculateXp(int attackerId, int defenderId, bool attackerWon)
        {
            var attacker = db.TeamMembers.Find(attackerId);
            var defender = db.TeamMembers.Find(defenderId);

            int levelDifference = attacker.Level - defender.Level;

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

            if (attackerWon)
            {
                AddXp(attackerId, winnerXp);
                AddXp(defenderId, loserXp);
            }

            else
            {
                AddXp(defenderId, winnerXp);
                AddXp(attackerId, loserXp);
            }
        }

        void AddXp(int teamMemberId, int xp)
        {
            var attacker = db.TeamMembers.Find(teamMemberId);

            if (attacker.Xp + xp >= attacker.MaxXp)
            {
                int remaingXp = attacker.MaxXp - attacker.Xp;

                attacker.Level++;
                attacker.Xp = xp - remaingXp;
            }

            else
            {
                attacker.Xp += xp;
            }

            db.SaveChanges();
        }
    }
}