using ClashOfTheCharacters.Models;
using ClashOfTheCharacters.Services;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    [Authorize]
    public class BattleController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var challenges = db.Challenges.Where(c => c.ChallengerId == userId || c.ReceiverId == userId && c.Accepted == false).ToList();
            var activeBattles = db.Battles.Where(b => b.Challenge.ChallengerId == userId && !b.Calculated).ToList();

            ViewBag.UserId = userId;
            ViewBag.Stamina = user.Stamina;
            ViewBag.Challenges = challenges;
            ViewBag.ActiveBattles = activeBattles;

            return View(db.Users.ToList());
        }

        public ActionResult Challenge(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Count == 0)
            {
                return RedirectToAction("Select", "Creature");
            }

            if (userId != id && id != null)
            {
                var activeChallenges = db.Challenges.Count(c => c.ChallengerId == userId && c.ReceiverId == id && c.Accepted == false);
                var activeBattles = db.Battles.Count(c => c.Challenge.ChallengerId == userId && c.Challenge.ReceiverId == id && c.Calculated == false);

                if (user.Stamina >= 6 && activeChallenges < 2 && activeBattles < 2)
                {
                    if (user.Stamina == user.MaxStamina)
                    {
                        user.LastStaminaTime = DateTimeOffset.Now;
                    }

                    user.Stamina -= 6;

                    var challenge = new Challenge { ChallengerId = userId, ReceiverId = id };

                    db.Challenges.Add(challenge);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Accept(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Count == 0)
            {
                return RedirectToAction("Select", "Creature");
            }

            if (!db.Battles.Any(b => b.ChallengeId == id))
            {
                var battle = new Battle { ChallengeId = id, StartTime = DateTime.Now.AddMinutes(2) };

                db.Challenges.Find(id).Accepted = true;
                db.Battles.Add(battle);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Cancel(int id)
        {
            var userId = User.Identity.GetUserId();

            if (db.Challenges.Any(c => c.Id == id))
            {
                var challenge = db.Challenges.Find(id);

                if (challenge.Challenger.Stamina != challenge.Challenger.MaxStamina)
                {
                    challenge.Challenger.Stamina += 6;
                }

                db.Challenges.Remove(challenge);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}