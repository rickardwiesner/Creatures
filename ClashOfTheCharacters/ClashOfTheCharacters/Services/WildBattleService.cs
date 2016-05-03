using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class WildBattleService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        Effect effect;

        public void InitializeBattle(string userId)
        {
            var wildBattle = db.WildBattles.FirstOrDefault(wb => wb.UserId == userId);

            if (wildBattle != null)
            {
                return;
            }

            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            db.WildBattles.Add(new WildBattle { StageId = currentLand.Land.Stages.First(s => s.Index == currentLand.CurrentStageIndex).Id, UserId = userId });
            db.SaveChanges();

            var numberOfCreatures = GetNumberOfCreatures(currentLand.CurrentStageIndex);
            var wildBattleId = db.WildBattles.First(ct => ct.UserId == userId).Id;

            for (int i = 1; i <= numberOfCreatures; i++)
            {
                var rarity = RandomizeRarity(currentLand.CurrentStageIndex);
                var creatureId = RandomizeCreatureId(rarity);

                var creature = db.Characters.Find(creatureId);

                var hp = Convert.ToInt32(currentLand.WildCreatureStartLevel * creature.HpMultiplier + creature.BaseHp);

                db.WildBattleCreatures.Add(new WildBattleCreature
                {
                    CharacterId = creatureId,
                    Hp = hp,
                    MaxHp = hp,
                    Slot = i,
                    Level = currentLand.WildCreatureStartLevel,
                    WildBattleId = wildBattleId,                
                });
            }

            var user = db.Users.Find(userId);

            foreach (var teamMember in user.TeamMembers.OrderBy(tm => tm.Slot).ToList())
            {
                db.WildBattleCreatures.Add(new WildBattleCreature
                {
                    TeamMemberId = teamMember.Id,
                    Level = teamMember.Level,
                    Hp = teamMember.Hp,
                    MaxHp = teamMember.Hp,
                    Slot = teamMember.Slot,
                    UserId = userId,
                    WildBattleId = wildBattleId,
                    CharacterId = teamMember.CharacterId                 
                });
            }

            db.SaveChanges();
        }

        public void Attack(string userId)
        {
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);

            var userCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId != null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();
            var cpuCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId == null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();

            int damage = Convert.ToInt32(CalculateDamage(userCreature.Id, cpuCreature.Id));
            int hpRemaining = cpuCreature.Hp - damage < 0 ? 0 : cpuCreature.Hp - damage;

            db.WildBattleActions.Add(new WildBattleAction
            {
                WildBattleId = wildBattle.Id,
                AttackerId = userCreature.Id,
                DefenderId = cpuCreature.Id,
                Damage = damage,
                Effect = effect,              
            });

            cpuCreature.Hp = hpRemaining;

            if (cpuCreature.Hp == 0 && !wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId == null && wbc.Alive))
            {
                //winner
                db.SaveChanges();

                return;
            }

            else if (cpuCreature.Hp == 0 && wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId == null && wbc.Alive))
            {        
                
                cpuCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId == null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();

            }

            damage = Convert.ToInt32(CalculateDamage(userCreature.Id, cpuCreature.Id));
            hpRemaining = cpuCreature.Hp - damage < 0 ? 0 : cpuCreature.Hp - damage;

            db.WildBattleActions.Add(new WildBattleAction
            {
                WildBattleId = wildBattle.Id,
                AttackerId = cpuCreature.Id,
                DefenderId = userCreature.Id,
                Damage = damage,
                Effect = effect
            });

            userCreature.Hp = hpRemaining;

            db.SaveChanges();
        }

        float CalculateDamage(int attackerId, int defenderId)
        {
            var attacker = db.WildBattleCreatures.Find(attackerId);
            var defender = db.WildBattleCreatures.Find(defenderId);

            float elementBonus = 1;
            effect = Effect.Normal;

            if (attacker.Character.Element == Element.Gravity && defender.Character.Element != Element.Gravity)
            {
                elementBonus = 1.25f;
                effect = Effect.GravityAttack;
            }

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == -2 || (int)defender.Character.Element - (int)attacker.Character.Element == 6)
            {
                elementBonus = 0.5f;
                effect = Effect.VeryBad;
            }

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == -1 || attacker.Character.Element == Element.Fire && defender.Character.Element == Element.Pollution)
            {
                elementBonus = 0.75f;
                effect = Effect.Bad;
            }

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == 1 || attacker.Character.Element == Element.Pollution && defender.Character.Element == Element.Fire)
            {
                elementBonus = 1.5f;
                effect = Effect.Good;
            }

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == 2 || (int)defender.Character.Element - (int)attacker.Character.Element == -6)
            {
                elementBonus = 2.0f;
                effect = Effect.VeryGood;
            }

            Random instance = new Random();

            float random = 0;

            while (random < 0.5)
            {
                random = (float)instance.NextDouble();
            }

            if (random > 0.95)
            {
                random = 2;
            }

            return (((2 * (float)attacker.Level + 10) / 250) * ((float)attacker.Damage / (float)defender.Defense) * (float)attacker.Character.BaseAttack + 2) * (1.5f * elementBonus * (random * 2));
        }

        int GetNumberOfCreatures(int stageIndex)
        {
            int numberOfCreatures = 0;

            if (stageIndex <= 2)
            {
                numberOfCreatures = 1;
            }

            else if (stageIndex <= 4)
            {
                numberOfCreatures = 2;
            }

            else
            {
                numberOfCreatures = 3;
            }

            return numberOfCreatures;
        }

        int RandomizeCreatureId(Rarity rarity)
        {
            var creatures = db.Characters.Where(c => c.Rarity == rarity).ToList();

            var instance = new Random();
            var random = instance.Next(creatures.Count());

            return creatures.ElementAt(random).Id;
        }

        Rarity RandomizeRarity(int stage)
        {
            var instance = new Random();
            var random = instance.Next(101);

            Rarity rarity;

            if (random == 0 && stage == 6)
            {
                rarity = Rarity.Legendary;
            }

            else if (random <= 5 && stage >= 5)
            {
                rarity = Rarity.Epic;
            }

            else if (random <= 15 && stage >= 3)
            {
                rarity = Rarity.Rare;
            }

            else if (random <= 50)
            {
                rarity = Rarity.Uncommon;
            }

            else
            {
                rarity = Rarity.Common;
            }

            return rarity;
        }
    }
}