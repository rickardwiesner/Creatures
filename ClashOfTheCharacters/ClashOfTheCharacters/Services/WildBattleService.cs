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
            //var wildBattle = db.WildBattles.FirstOrDefault(wb => wb.UserId == userId);

            //if (wildBattle != null)
            //{
            //    return;
            //}

            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            db.WildBattles.Add(new WildBattle { StageId = currentLand.Land.Stages.First(s => s.Index == currentLand.CurrentStageIndex).Id, UserId = userId });
            db.SaveChanges();

            var numberOfCreatures = GetNumberOfCreatures(currentLand.CurrentStageIndex);
            var wildBattleId = db.WildBattles.First(ct => ct.UserId == userId).Id;

            for (int i = 1; i <= numberOfCreatures; i++)
            {
                Rarity rarity = new Rarity();

                for (int j = 0; j < 10; j++)
                {
                    rarity = RandomizeRarity(currentLand.CurrentStageIndex, currentLand.Land.Stages.Max(s => s.Index));
                }

                int creatureId = 0;

                for (int j = 0; j < 10; j++)
                {
                    creatureId = RandomizeCreatureId(rarity);
                }

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
                wildBattle.Finished = true;
                wildBattle.Won = true;

                db.SaveChanges();

                return;
            }

            else if (cpuCreature.Hp == 0 && wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId == null && wbc.Alive))
            {        
                
                cpuCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId == null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();

            }

            damage = Convert.ToInt32(CalculateDamage(cpuCreature.Id, userCreature.Id));
            hpRemaining = userCreature.Hp - damage < 0 ? 0 : userCreature.Hp - damage;

            db.WildBattleActions.Add(new WildBattleAction
            {
                WildBattleId = wildBattle.Id,
                AttackerId = cpuCreature.Id,
                DefenderId = userCreature.Id,
                Damage = damage,
                Effect = effect
            });

            userCreature.Hp = hpRemaining;

            if (userCreature.Hp == 0 && !wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId != null && wbc.Alive))
            {
                wildBattle.Finished = true;
            }

            db.SaveChanges();
        }

        public void FinishBattle(string userId)
        {
            var user = db.Users.Find(userId);
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);
            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            if (wildBattle.Won)
            {
                var experienceService = new ExperienceService();

                if (user.ClearedLands.Any(cl => cl.LandId == currentLand.LandId))
                {
                    user.Gold += (wildBattle.Stage.GoldReward / 2);
                    experienceService.AddXp(userId, (wildBattle.Stage.XpReward / 2));
                }

                else
                {
                    user.Gold += wildBattle.Stage.GoldReward;
                    experienceService.AddXp(userId, wildBattle.Stage.XpReward);
                }

                if (wildBattle.Stage.Index == currentLand.Land.Stages.Max(s => s.Index))
                {
                    if (!db.ClearedLands.Any(cl => cl.UserId == userId && cl.LandId == currentLand.LandId))
                    {
                        db.ClearedLands.Add(new ClearedLand { UserId = userId, LandId = currentLand.LandId });
                    }

                    db.CurrentLands.Remove(currentLand);
                }

                else
                {
                    currentLand.CurrentStageIndex++;
                }
            }

            db.WildBattles.Remove(wildBattle);

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
            int random = 0;

            for (int i = 0; i < 10; i++)
            {
                random = instance.Next(creatures.Count());
            }

            return creatures.ElementAt(random).Id;
        }

        Rarity RandomizeRarity(int stage, int maxStage)
        {
            var instance = new Random();
            var random = instance.NextDouble();

            Rarity rarity;

            if (random <= 0.01 && stage == maxStage)
            {
                rarity = Rarity.Legendary;
            }

            else if (random <= 0.05 && maxStage - stage <= 1)
            {
                rarity = Rarity.Epic;
            }

            else if (random <= 0.15 && maxStage - stage <= 3)
            {
                rarity = Rarity.Rare;
            }

            else if (random <= 0.5)
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