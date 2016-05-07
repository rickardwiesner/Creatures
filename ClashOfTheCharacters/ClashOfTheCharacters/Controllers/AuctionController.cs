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
            var userId = User.Identity.GetUserId();
            ViewBag.Gold = db.Users.Find(userId).Gold;
            ViewBag.User = db.Users.Find(userId);

            return View("Index", db.AuctionCreatures.Where(ac => !ac.Finished && ac.OwnerId != userId).OrderBy(ac => ac.EndTime).ToList());
        }

        public ActionResult Selling()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Gold = db.Users.Find(userId).Gold;
            ViewBag.User = db.Users.Find(userId);

            return View("Index", db.AuctionCreatures.Where(ac => ac.OwnerId == userId).ToList());
        }

        public ActionResult Targets()
        {
            var userId = User.Identity.GetUserId();
            ViewBag.Gold = db.Users.Find(userId).Gold;
            ViewBag.User = db.Users.Find(userId);

            var auctionTargets = db.AuctionTargets.Where(at => at.UserId == userId).Select(at => at.AuctionCreatureId);

            return View("Index", db.AuctionCreatures.Where(ac => auctionTargets.Contains(ac.Id)).ToList());
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

            if (auctionCreature.CurrentBid != null && amount >= auctionCreature.CurrentBid + 50 || auctionCreature.CurrentBid == null && user.Gold >= amount && !user.UserCreatures.Any(uc => uc.CreatureId == auctionCreature.UserCreature.CreatureId))
            {
                if (auctionCreature.CurrentBidder != null)
                {
                    auctionCreature.CurrentBidder.Gold += (int)auctionCreature.CurrentBid;
                }

                user.Gold -= amount;
                auctionCreature.CurrentBid = amount;
                auctionCreature.CurrentBidderId = userId;

                if ((auctionCreature.EndTime - DateTimeOffset.Now).TotalSeconds < 30)
                {
                    auctionCreature.EndTime = DateTimeOffset.Now.AddSeconds(30);
                }

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
                if (auctionCreature.CurrentBidder != null)
                {
                    auctionCreature.CurrentBidder.Gold += (int)auctionCreature.CurrentBid;
                }

                auctionCreature.Owner.Gold += Convert.ToInt32((float)auctionCreature.BuyoutPrice * 0.95f);
                user.Gold -= (int)auctionCreature.BuyoutPrice;
                auctionCreature.UserCreature.UserId = userId;
                auctionCreature.UserCreature.InAuction = false;
                auctionCreature.Finished = true;
                auctionCreature.CurrentBidder = user;
                auctionCreature.CurrentBid = (int)auctionCreature.BuyoutPrice;

                if (!user.AuctionTargets.Any(at => at.AuctionCreatureId == auctionCreatureId))
                {
                    db.AuctionTargets.Add(new AuctionTarget { AuctionCreatureId = auctionCreatureId, UserId = userId });
                }

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
    }
}