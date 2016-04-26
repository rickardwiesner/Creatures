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
                    int hpRemaining = receiverCharacter.Hp - damage < 0 ? 0 : receiverCharacter.Hp - damage;

                    db.Attacks.Add(new Attack
                    {
                        BattleId = battleId,
                        AttackerId = challengerCharacter.Id,
                        DefenderId = receiverCharacter.Id,
                        Damage = damage,
                        AttackerHpRemaining = challengerCharacter.Hp,
                        DefenderHpRemaining = hpRemaining,
                        Effect = effect
                    });

                    receiverCharacter.Hp = hpRemaining;

                    if (receiverCharacter.Alive)
                    {
                        damage = Convert.ToInt32(CalculateDamage(receiverCharacter.Id, challengerCharacter.Id));
                        hpRemaining = challengerCharacter.Hp - damage < 0 ? 0 : challengerCharacter.Hp - damage;

                        db.Attacks.Add(new Attack
                        {
                            BattleId = battleId,
                            AttackerId = receiverCharacter.Id,
                            DefenderId = challengerCharacter.Id,
                            Damage = damage,
                            AttackerHpRemaining = receiverCharacter.Hp,
                            DefenderHpRemaining = hpRemaining,
                            Effect = effect
                        });

                        challengerCharacter.Hp = hpRemaining;
                    }

                    //else if han har någon annan gubbe som lever
                }     

                var experienceService = new ExperienceService();
                experienceService.CalculateXp
                    (
                    challengerCompetitor.User.TeamMembers.First(tm => tm.CharacterId == challengerCharacter.TeamMember.CharacterId).Id,
                    receiverCompetitor.User.TeamMembers.First(tm => tm.CharacterId == receiverCharacter.TeamMember.CharacterId).Id,
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

            if (attacker.TeamMember.Character.Element == Element.Gravity && defender.TeamMember.Character.Element != Element.Gravity)
            {
                elementBonus = 1.25f;
                effect = Effect.GravityAttack;
            }

            else if ((int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == -2 || (int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == 6)
            {
                elementBonus = 0.5f;
                effect = Effect.VeryBad;
            }

            else if ((int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == -1 || attacker.TeamMember.Character.Element == Element.Fire && defender.TeamMember.Character.Element == Element.Polution)
            {
                elementBonus = 0.75f;
                effect = Effect.Bad;
            }

            else if ((int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == 1 || attacker.TeamMember.Character.Element == Element.Polution && defender.TeamMember.Character.Element == Element.Fire)
            {
                elementBonus = 1.5f;
                effect = Effect.Good;
            }

            else if ((int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == 2 || (int)defender.TeamMember.Character.Element - (int)attacker.TeamMember.Character.Element == -6)
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

            //int random = instance.Next(4);

            //float random = (float)instance.NextDouble();

            return (((2 * (float)attacker.Level + 10) / 250) * ((float)attacker.Damage / (float)defender.Defense) * (float)attacker.TeamMember.Character.BaseAttack + 2) * (1.5f * elementBonus * (random * 2));
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
                    TeamMemberId = teamMember.Id,
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
                    TeamMemberId = teamMember.Id,
                    Level = teamMember.Level,
                    Hp = teamMember.Hp,
                    MaxHp = teamMember.Hp
                });
            }

            db.SaveChanges();
        }
    }
}