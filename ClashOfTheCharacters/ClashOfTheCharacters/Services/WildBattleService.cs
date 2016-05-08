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
        ExperienceService experienceService = new ExperienceService();
        Effect effect;

        public void InitializeBattle(string userId)
        {
            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            db.WildBattles.Add(new WildBattle { LandId = currentLand.LandId, UserId = userId });
            db.SaveChanges();

            var numberOfCreatures = GetNumberOfCreatures(currentLand.CurrentLevel);
            var wildBattleId = db.WildBattles.First(ct => ct.UserId == userId).Id;

            for (int i = 1; i <= numberOfCreatures; i++)
            {
                Rarity rarity = new Rarity();

                for (int j = 0; j < 10; j++)
                {
                    rarity = RandomizeRarity(currentLand.CurrentLevel, currentLand.Land.Levels);
                }

                int creatureId = 0;

                for (int j = 0; j < 10; j++)
                {
                    creatureId = RandomizeCreatureId(rarity);
                }

                var creature = db.Creatures.Find(creatureId);

                var hp = Convert.ToInt32(currentLand.WildCreatureStartLevel * creature.HpMultiplier + creature.BaseHp);

                db.WildBattleCreatures.Add(new WildBattleCreature
                {
                    CreatureId = creatureId,
                    Hp = hp,
                    MaxHp = hp,
                    Slot = i,
                    Level = currentLand.WildCreatureStartLevel,
                    WildBattleId = wildBattleId,                
                });
            }

            var user = db.Users.Find(userId);

            foreach (var userCreature in user.UserCreatures.Where(uc => uc.InSquad).OrderBy(tm => tm.Slot).ToList())
            {
                db.WildBattleCreatures.Add(new WildBattleCreature
                {
                    Level = userCreature.Level,
                    Hp = userCreature.Hp,
                    MaxHp = userCreature.Hp,
                    Slot = userCreature.Slot,
                    UserId = userId,
                    WildBattleId = wildBattleId,
                    CreatureId = userCreature.CreatureId                 
                });
            }

            db.SaveChanges();
        }

        public void Attack(string userId)
        {
            var user = db.Users.Find(userId);

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

            if (cpuCreature.Hp == 0)
            {
                experienceService.CalculateWildXp(userCreature.Id, cpuCreature.Level, true);
            }

            if (!wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId == null && wbc.Alive))
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

            if (userCreature.Hp == 0)
            {
                experienceService.CalculateWildXp(userCreature.Id, cpuCreature.Level, false);
            }

            if (!wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId != null && wbc.Alive))
            {
                wildBattle.Finished = true;
            }

            db.SaveChanges();
        }

        public void Capture(string userId, int userItemId)
        {
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);

            var userCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId != null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();
            var cpuCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId == null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();

            if (userCreature.User.UserCreatures.Any(uc => uc.CreatureId == cpuCreature.CreatureId))
            {
                return;
            }

            var result = CaptureAttempt(userId, cpuCreature.Id, userItemId);
            effect = result == true ? Effect.Success : Effect.Fail;

            db.WildBattleActions.Add(new WildBattleAction
            {
                WildBattleId = wildBattle.Id,
                DefenderId = cpuCreature.Id,
                Effect = effect,
                CaptureAttempt = true,
                CaptureSuccess = result
            });

            cpuCreature.Hp = result == true ? 0 : cpuCreature.Hp;

            var userItem = db.UserItems.Find(userItemId);

            if (userItem.Quantity == 1)
            {
                db.UserItems.Remove(userItem);
            }

            else
            {
                userItem.Quantity--;
            }

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

            int damage = Convert.ToInt32(CalculateDamage(cpuCreature.Id, userCreature.Id));
            int hpRemaining = userCreature.Hp - damage < 0 ? 0 : userCreature.Hp - damage;

            db.WildBattleActions.Add(new WildBattleAction
            {
                WildBattleId = wildBattle.Id,
                AttackerId = cpuCreature.Id,
                DefenderId = userCreature.Id,
                Damage = damage,
                Effect = effect
            });

            userCreature.Hp = hpRemaining;

            if (userCreature.Hp == 0)
            {
                experienceService.CalculateWildXp(userCreature.Id, cpuCreature.Level, false);
            }

            if (userCreature.Hp == 0 && !wildBattle.WildBattleCreatures.Any(wbc => wbc.UserId != null && wbc.Alive))
            {
                wildBattle.Finished = true;
            }

            db.SaveChanges();
        }

        bool CaptureAttempt(string userId, int cpuCreatureId, int userItemId)
        {
            var user = db.Users.Find(userId);
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);
            var userItem = db.UserItems.Find(userItemId);
            var cpuCreature = wildBattle.WildBattleCreatures.Where(wbc => wbc.UserId == null && wbc.Alive).OrderBy(wbc => wbc.Slot).First();
            var levelDifference = user.Level - cpuCreature.Level;

            float successrate = 1f;

            if (levelDifference < -10)
            {
                successrate = 0.01f;
            }

            else if (levelDifference < -5)
            {
                successrate = 0.1f;
            }

            else if (levelDifference < -1)
            {
                successrate = 0.25f;
            }

            else if (levelDifference >= -1 && levelDifference <= 1)
            {
                successrate = 0.5f;
            }

            else if (levelDifference < 5)
            {
                successrate = 0.75f;
            }

            switch (cpuCreature.Creature.Rarity)
            {
                case Rarity.Uncommon:
                    successrate -= 0.1f;
                    break;

                case Rarity.Rare:
                    successrate -= 0.2f;
                    break;

                case Rarity.Epic:
                    successrate -= 0.5f;
                    break;

                case Rarity.Legendary:
                    successrate -= 0.9f;
                    break;
            }

            switch (userItem.Item.Rarity)
            {
                case Rarity.Uncommon:
                    successrate += 0.1f;
                    break;

                case Rarity.Rare:
                    successrate += 0.3f;
                    break;

                case Rarity.Epic:
                    successrate += 0.6f;
                    break;

                case Rarity.Legendary:
                    successrate = 1;
                    break;
            }

            var hpRemaining = (float)cpuCreature.Hp / cpuCreature.MaxHp;
            successrate += successrate - (successrate * hpRemaining);

            var instance = new Random();
            var random = instance.NextDouble();

            return random < successrate;
        }

        public void FinishBattle(string userId)
        {
            var user = db.Users.Find(userId);
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);
            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            if (wildBattle.Won)
            {
                foreach (var wildBattleAction in wildBattle.WildBattleActions.Where(wb => wb.CaptureSuccess == true))
                {
                    db.UserCreatures.Add(new UserCreature
                    {
                        CreatureId = wildBattleAction.Defender.CreatureId,
                        Level = wildBattleAction.Defender.Level,
                        UserId = userId
                    });
                }

                var experienceService = new ExperienceService();

                if (user.ClearedLands.Any(cl => cl.LandId == currentLand.LandId))
                {
                    user.Gold += (wildBattle.Land.GoldReward / 2);
                    experienceService.AddXp(userId, (wildBattle.Land.XpReward / 2));
                }

                else
                {
                    user.Gold += wildBattle.Land.GoldReward;
                    experienceService.AddXp(userId, wildBattle.Land.XpReward);
                }

                if (currentLand.CurrentLevel == currentLand.Land.Levels)
                {
                    if (!db.ClearedLands.Any(cl => cl.UserId == userId && cl.LandId == currentLand.LandId))
                    {
                        db.ClearedLands.Add(new ClearedLand { UserId = userId, LandId = currentLand.LandId });
                    }

                    db.CurrentLands.Remove(currentLand);
                }

                else
                {
                    currentLand.CurrentLevel++;
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

            if (attacker.Creature.Element == Element.Gravity && defender.Creature.Element != Element.Gravity)
            {
                elementBonus = 1.25f;
                effect = Effect.GravityAttack;
            }

            else if ((int)defender.Creature.Element - (int)attacker.Creature.Element == -2 || (int)defender.Creature.Element - (int)attacker.Creature.Element == 6)
            {
                elementBonus = 0.5f;
                effect = Effect.VeryBad;
            }

            else if ((int)defender.Creature.Element - (int)attacker.Creature.Element == -1 || attacker.Creature.Element == Element.Fire && defender.Creature.Element == Element.Pollution)
            {
                elementBonus = 0.75f;
                effect = Effect.Bad;
            }

            else if ((int)defender.Creature.Element - (int)attacker.Creature.Element == 1 || attacker.Creature.Element == Element.Pollution && defender.Creature.Element == Element.Fire)
            {
                elementBonus = 1.5f;
                effect = Effect.Good;
            }

            else if ((int)defender.Creature.Element - (int)attacker.Creature.Element == 2 || (int)defender.Creature.Element - (int)attacker.Creature.Element == -6)
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

            return (((2 * (float)attacker.Level + 10) / 250) * ((float)attacker.Damage / (float)defender.Defense) * (float)attacker.Creature.BaseAttack + 2) * (1.5f * elementBonus * (random * 2));
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
            var creatures = db.Creatures.Where(c => c.Rarity == rarity).ToList();

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