using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClashOfTheCharacters.Services
{
    public class BattleService
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private Effect effect;
        private int battleId;

        public void RunBattles()
        {
            foreach (var battle in db.Battles.Where(b => b.Calculated != true).ToList())
            {
                if (battle.Aired == true)
                {
                    battleId = battle.Id;
                    AddCompetitors();
                    AddBattleCharacters();
                    RunBattle();
                    battle.Calculated = true;
                }
            }

            db.SaveChanges();
        }

        public void RunBattle()
        {
            var battle = db.Battles.Find(battleId);

            var challengerCompetitor = battle.Competitors.First(c => c.Challenger);
            var receiverCompetitor = battle.Competitors.First(c => !c.Challenger);

            //var challengerCharacters = challengerCompetitor.BattleCharacters;
            //var receiverCharacters = receiverCompetitor.BattleCharacters;

            //Så länge båda tävlande har Hp kvar...
            while (challengerCompetitor.TotalHp > 0 && receiverCompetitor.TotalHp > 0)
            {
                //så plockar vi fram första karatären i båda lagen som lever.
                var challengerCharacter = challengerCompetitor.BattleCharacters.First(ct => ct.Alive);
                var receiverCharacter = receiverCompetitor.BattleCharacters.First(rt => rt.Alive);

                //Så länge båda av dom lever...
                while (challengerCharacter.Alive && receiverCharacter.Alive)
                {
                    int damage = Convert.ToInt32(CalculateDamage(challengerCharacter.Id, receiverCharacter.Id));

                    db.Attacks.Add(new Attack
                    {
                        BattleId = battleId,
                        AttackerId = challengerCharacter.Id,
                        DefenderId = receiverCharacter.Id,
                        Damage = damage,
                        HpRemaining = receiverCharacter.Hp - damage,
                        Effect = effect
                    });

                    receiverCharacter.Hp -= damage;

                    if (receiverCharacter.Alive)
                    {
                        damage = Convert.ToInt32(CalculateDamage(receiverCharacter.Id, challengerCharacter.Id));

                        db.Attacks.Add(new Attack
                        {
                            BattleId = battleId,
                            AttackerId = receiverCharacter.Id,
                            DefenderId = challengerCharacter.Id,
                            Damage = damage,
                            HpRemaining = challengerCharacter.Hp - damage,
                            Effect = effect
                        });

                        challengerCharacter.Hp -= damage;
                    }

                    //else if han har någon annan gubbe som lever
                }     

                var experienceService = new ExperienceService();
                experienceService.CalculateXp
                    (
                    challengerCompetitor.User.TeamMembers.First(tm => tm.CharacterId == challengerCharacter.CharacterId).Id,
                    receiverCompetitor.User.TeamMembers.First(tm => tm.CharacterId == receiverCharacter.CharacterId).Id,
                    challengerCharacter.Alive
                    );
            }

            if (receiverCompetitor.TotalHp > 0)
            {
                receiverCompetitor.Winner = true;
            }

            else
            {
                challengerCompetitor.Winner = true;
            }

            db.SaveChanges();

            var rewardService = new RewardService();
            rewardService.DistributeReward(battleId);          
        }

        float CalculateDamage(int attackerId, int defenderId)
        {
            var attacker = db.BattleCharacters.Find(attackerId);
            var defender = db.BattleCharacters.Find(defenderId);

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

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == -1 || attacker.Character.Element == Element.Fire && defender.Character.Element == Element.Polution)
            {
                elementBonus = 0.75f;
                effect = Effect.Bad;
            }

            else if ((int)defender.Character.Element - (int)attacker.Character.Element == 1 || attacker.Character.Element == Element.Polution && defender.Character.Element == Element.Fire)
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

            while (random < 0.85)
            {
                random = (float)instance.NextDouble();
            }

            return (((2 * (float)attacker.Level + 10) / 250) * ((float)attacker.Damage / (float)defender.Defense) * (float)attacker.Character.BaseAttack + 2) * (1.5f * elementBonus * random);
        }

        void AddCompetitors()
        {
            var battle = db.Battles.Find(battleId);

            var challengerId = battle.Challenge.ChallengerId;
            var receiverId = battle.Challenge.ReceiverId;

            db.Competitors.Add(new Competitor { BattleId = battle.Id, UserId = challengerId, Challenger = true });
            db.Competitors.Add(new Competitor { BattleId = battle.Id, UserId = receiverId });

            db.SaveChanges();
        }

        void AddBattleCharacters()
        {
            var battle = db.Battles.Find(battleId);

            foreach (var teamMember in battle.Challenge.Challenger.TeamMembers.ToList())
            {
                db.BattleCharacters.Add(new BattleCharacter
                {
                    CompetitorId = battle.Competitors.First(c => c.Challenger).Id,
                    CharacterId = teamMember.CharacterId,
                    Level = teamMember.Level,
                    Hp = teamMember.Hp,
                    MaxHp = teamMember.Hp
                });
            }

            foreach (var teamMember in battle.Challenge.Receiver.TeamMembers.ToList())
            {
                db.BattleCharacters.Add(new BattleCharacter
                {
                    CompetitorId = battle.Competitors.First(c => !c.Challenger).Id,
                    CharacterId = teamMember.CharacterId,
                    Level = teamMember.Level,
                    Hp = teamMember.Hp,
                    MaxHp = teamMember.Hp
                });
            }

            db.SaveChanges();
        }
    }
}