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
            var battles = db.Battles.Where(b => b.Challenge.ChallengerId == userId || b.Challenge.ReceiverId == userId).ToList();

            var battleService = new BattleService();
            battleService.RunBattles();

            if (user.Stamina < user.MaxStamina)
            {
                var staminaService = new StaminaService();
                staminaService.UpdateStamina(userId);
            }

            ViewBag.UserId = userId;
            ViewBag.Challenges = challenges;
            ViewBag.Battles = battles;

            return View(db.Users.ToList());
        }

        public ActionResult Challenge(string id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.TeamMembers.Count == 0)
            {
                return RedirectToAction("Select", "Character");
            }

            if (userId != id && id != null)
            {

                if (user.Stamina >= 6 && db.Challenges.Count(c => c.ChallengerId == userId && c.ReceiverId == id && c.Accepted == false) < 2)
                {
                    user.Stamina -= 6;

                    var challenge = new Challenge { ChallengerId = userId, ReceiverId = id };

                    db.Challenges.Add(challenge);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Accept(int challengeId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.TeamMembers.Count == 0)
            {
                return RedirectToAction("Select", "Character");
            }

            var battle = new Battle { ChallengeId = challengeId, StartTime = DateTime.Now.AddMinutes(2) };

            db.Challenges.Find(challengeId).Accepted = true;
            db.Battles.Add(battle);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Cancel(int challengeId)
        {
            var userId = User.Identity.GetUserId();

            var challenge = db.Challenges.Find(challengeId);
            challenge.Challenger.Stamina += 6;

            db.Challenges.Remove(challenge);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}