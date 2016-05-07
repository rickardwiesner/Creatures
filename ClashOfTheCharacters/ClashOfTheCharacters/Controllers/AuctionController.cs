using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Models;
using ClashOfTheCharacters.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
                
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Creatures()
        {
            return View(db.AuctionCreatures.ToList());
        }

        public ActionResult Items()
        {
            return View(db.AuctionItems.ToList());
        }

        [HttpPost]
        public ActionResult BidCreature()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var auctionCreatureId = Convert.ToInt32(Request.Form.Get("auctionCreatureId"));
            var amount = Convert.ToInt32(Request.Form.Get("amount"));

            var auctionCreature = db.AuctionCreatures.Find(auctionCreatureId);

            if (user.Gold >= amount && !user.UserCreatures.Any(uc => uc.CreatureId == auctionCreature.UserCreature.CreatureId))
            {
                auctionCreature.CurrentBid = amount;
                auctionCreature.CurrentBidderId = userId;

                if (!user.AuctionTargets.Any(at => at.AuctionCreatureId == auctionCreatureId))
                {
                    db.AuctionTargets.Add(new AuctionTarget { AuctionCreatureId = auctionCreatureId, UserId = userId });
                }

                db.SaveChanges();
            }

            return RedirectToAction("Creatures");
        }

        [HttpPost]
        public ActionResult BuyoutCreature()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            var auctionCreatureId = Convert.ToInt32(Request.Form.Get("auctionCreatureId"));

            var auctionCreature = db.AuctionCreatures.Find(auctionCreatureId);

            if (auctionCreature.BuyoutPrice != null && user.Gold >= auctionCreature.BuyoutPrice && !user.UserCreatures.Any(uc => uc.Id == auctionCreature.UserCreatureId))
            {
                auctionCreature.Owner.Gold += Convert.ToInt32((float)auctionCreature.BuyoutPrice * 0.95f);
                user.Gold -= (int)auctionCreature.BuyoutPrice;
                auctionCreature.UserCreature.UserId = userId;
                auctionCreature.UserCreature.InAuction = false;

                db.AuctionTargets.Add(new AuctionTarget { AuctionCreatureId = auctionCreatureId, UserId = userId });

                db.SaveChanges();
            }


            return RedirectToAction("Creatures");
        }

        public ActionResult SellCreature(int userCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Any(uc => uc.Id == userCreatureId && !uc.InSquad) && user.UserCreatures.Count(uc => !uc.InAuction) > 1)
            {
                var userCreature = user.UserCreatures.First(uc => uc.Id == userCreatureId);
                var auctionCreatureViewModel = new AuctionCreatureViewModel { UserCreatureId = userCreatureId, Name = userCreature.Creature.Name, StartPrice = userCreature.Worth, Hours = 1 };

                return View(auctionCreatureViewModel);
            };

            return RedirectToAction("Index", "Creature");
        }

        [HttpPost]
        public ActionResult SellCreature(AuctionCreatureViewModel auctionCreatureViewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);

                if (user.UserCreatures.Any(uc => uc.Id == auctionCreatureViewModel.UserCreatureId))
                {
                    db.AuctionCreatures.Add(new AuctionCreature
                    {
                        OwnerId = userId,
                        UserCreatureId = auctionCreatureViewModel.UserCreatureId,
                        StartPrice = auctionCreatureViewModel.StartPrice,
                        BuyoutPrice = auctionCreatureViewModel.BuyoutPrice,
                        EndTime = DateTimeOffset.Now.AddHours(auctionCreatureViewModel.Hours)
                    });

                    user.UserCreatures.First(uc => uc.Id == auctionCreatureViewModel.UserCreatureId).InAuction = true;

                    db.SaveChanges();
                }

                return RedirectToAction("Index", "Creature");
            }

            return View();
        }

        public ActionResult Selling()
        {
            var userId = User.Identity.GetUserId();

            return View("Creatures", db.AuctionCreatures.Where(ac => ac.OwnerId == userId).ToList());
        }

        public ActionResult Targets()
        {
            var userId = User.Identity.GetUserId();

            return View(db.AuctionTargets.Where(at => at.UserId == userId).ToList());
        }
    }
}