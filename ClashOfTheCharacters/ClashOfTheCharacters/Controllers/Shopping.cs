using ClashOfTheCharacters.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClashOfTheCharacters.ViewModels;
using ClashOfTheCharacters.Helpers;

namespace ClashOfTheCharacters.Controllers
{
    public class ShoppingController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Shopping
        [Authorize]
        public ActionResult Index(string sortOrder)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            //var userCharacters = user.UserCreatures.Select(x => x.CreatureId);
            //var characters = db.Creatures.Where(x => !userCharacters.Contains(x.Id) && x.Rarity != Rarity.Legendary && x.Rarity != Rarity.Epic).ToList();

            var sellShop = new SellShopView()
            {
                ShoppingItems = db.Creatures.Where(c => /*!userCharacters.Contains(x.Id) && */c.Rarity != Rarity.Legendary && c.Rarity != Rarity.Epic).OrderBy(c => c.Price).ToList(),
                //SellItems = user.UserCreatures.Select(o => o.Creature).ToList(),
                UserCreatures = user.UserCreatures,
                Gold = user.Gold,
                RainbowGems = user.RainbowGems,
                Stamina = user.Stamina,
                MaxStamina = user.MaxStamina,
                Travelling = db.Travels.Any(wb => wb.UserId == userId) || db.CurrentLands.Any(cl => cl.UserId == userId)
            };

            return View(sellShop);
        }
        [HttpPost, ActionName("Buy")]
        [ValidateAntiForgeryToken]
        public ActionResult BuyConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var gold = user.Gold;
            var characterPrice = db.Creatures.Find(id).Price;

            if (gold >= characterPrice)
            {
                db.UserCreatures.Add(new UserCreature { UserId = userId, CreatureId = id, Level = 1 });
                user.Gold -= characterPrice;
                db.SaveChanges();
                db.Dispose();
            }

            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Sell")]
        [ValidateAntiForgeryToken]
        public ActionResult SellingConfirmed(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Count() > 1)
            {
                var userCreature = user.UserCreatures.Single(o => o.CreatureId == id);
                var worth = userCreature.Worth;

                user.Gold += worth;
                db.UserCreatures.Remove(userCreature);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult FullStamina()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.RainbowGems >= 1)
            {
                user.Stamina = user.MaxStamina;
                user.RainbowGems -= 1;

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}