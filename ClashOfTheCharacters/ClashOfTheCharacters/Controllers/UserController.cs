using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet;
using Microsoft.AspNet.Identity;
using ClashOfTheCharacters.ViewModels;
using ClashOfTheCharacters.Services;

namespace ClashOfTheCharacters.Controllers
{
    public class UserController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var userViewModel = new UserViewModel
            {
                Username = user.UserName,
                ProfilePicture = user.ImageUrl,
                WonBattles = user.Wins,
                LostBattles = user.Losses,
                MostUsedCreature = user.UserCreatures.OrderByDescending(uc => uc.Battles).First(),
                MostValuableCreature = user.UserCreatures.OrderByDescending(uc => uc.Kills).First()
            };

            return View(userViewModel);
        }

        public ActionResult RemoveFromSquad(int userCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Any(uc => uc.Id == userCreatureId && uc.InSquad) && !db.Travels.Any(t => t.UserId == userId) && !db.CurrentLands.Any(cl => cl.UserId == userId) && user.UserCreatures.Count(uc => uc.InSquad) > 1)
            {
                var userCreature = user.UserCreatures.First(uc => uc.Id == userCreatureId);
                userCreature.InSquad = false;
                userCreature.Slot = 0;

                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult ChangeSlot(int originalUserCreatureId, int replacerUserCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var originalTeamMember = user.UserCreatures.First(uc => uc.Id == originalUserCreatureId);
            int originalTeamMemberSlot = originalTeamMember.Slot;

            var replacerTeamMember = user.UserCreatures.First(uc => uc.Id == replacerUserCreatureId);
            int replacerTeamMemberSlot = replacerTeamMember.Slot;

            originalTeamMember.Slot = replacerTeamMemberSlot;
            replacerTeamMember.Slot = originalTeamMemberSlot;

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        public ActionResult ChangeToEmptySlot(int userCreatureId, int slot)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            user.UserCreatures.First(tm => tm.Id == userCreatureId).Slot = slot;

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        [ChildActionOnly]
        public ActionResult ProfilePartial()
        {
            RunServices();

            var db = new ApplicationDbContext();

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            int index = 0;

            foreach (var u in db.Users.OrderByDescending(u => u.LadderPoints))
            {
                index++;

                if (u.Id == userId)
                {
                    break;
                }
            }

            var profilePartialViewModel = new ProfilePartialViewModel
            {
                UserId = userId,
                Username = user.UserName,
                ImageUrl = user.ImageUrl == null ? "/Images/Other/noprofilepicture.jpg" : user.ImageUrl,
                Rank = index,
                Gold = user.Gold,
                LadderPoints = user.LadderPoints,
                Stamina = user.Stamina,
                MaxStamina = user.MaxStamina,
                Xp = user.Xp,
                MaxXp = user.MaxXp,
                Level = user.Level,
                TotalUserCreatures = user.UserCreatures.Count,
                RainbowGems = user.RainbowGems,
                Travelling = db.Travels.Any(t => t.UserId == userId) || db.CurrentLands.Any(cl => cl.UserId == userId),
                NextStaminaMinutes = 10 - (DateTimeOffset.Now - user.LastStaminaTime).Minutes,
                UserCreatures = user.UserCreatures.Where(uc => uc.InSquad).OrderBy(uc => uc.Slot).ToList(),
                RecentBattles = db.Battles.Where(b => (b.Challenge.ChallengerId == userId || b.Challenge.ReceiverId == userId) && b.Calculated).OrderByDescending(b => b.StartTime).Take(10).ToList(),
                OngoingBattles = db.Battles.Where(b => (b.Challenge.ChallengerId == userId || b.Challenge.ReceiverId == userId) && !b.Calculated).ToList()
            };

            return PartialView("_ProfilePartial", profilePartialViewModel);
        }

        [HttpPost]
        public ActionResult UpdateProfilePicture()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var imageUrl = Request.Form.Get("imageUrl");

            user.ImageUrl = imageUrl;
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.PathAndQuery);
        }

        void RunServices()
        {
            var db = new ApplicationDbContext();

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if ((DateTimeOffset.Now - user.LastRainbowGemTime).TotalHours >= 24 )
            {
                user.RainbowGems++;
                user.LastRainbowGemTime = DateTimeOffset.Now;
            }

            user.LastActive = DateTimeOffset.Now;
            db.SaveChanges();

            if (db.Travels.Any(t => t.UserId == userId))
            {
                var travelService = new TravelService();
                travelService.CheckArrivalTime(userId);
            }

            if (user.Stamina < user.MaxStamina)
            {
                var staminaService = new StaminaService();
                staminaService.UpdateStamina(userId);
            }

            var battleService = new BattleService();
            battleService.RunBattles();

            var auctionService = new AuctionService();
            auctionService.CheckAuctions();
        }
    }
}