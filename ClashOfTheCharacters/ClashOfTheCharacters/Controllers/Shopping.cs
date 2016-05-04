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
            var userCharacters = user.TeamMembers.Select(x => x.CharacterId);
            var characters = db.Characters.Where(x => !userCharacters.Contains(x.Id) && x.Rarity != Rarity.Legendary && x.Rarity != Rarity.Epic).ToList();

            var sellShop = new SellShopView()
            {
                ShoppingItems = characters,
                SellItems = user.TeamMembers.Select(o => o.Character).ToList(),
                Gold = user.Gold,
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
            var characterPrice = db.Characters.Find(id).Price;

            int slot = 0;

            for (int i = 1; i <= 6; i++)
            {
                if (!user.TeamMembers.Any(tm => tm.Slot == i))
                {
                    slot = i;
                    break;
                }
            }

            //var slotsTaken = user.TeamMembers.Select(tm => tm.Slot);
            //var slot = slotsTaken.Contains(1) ? slotsTaken.First(st => st + 1 != st++) : 1;

            if (gold >= characterPrice && user.TeamMembers.Count < 6)
            {
                db.TeamMembers.Add(new TeamMember { ApplicationUserId = userId, CharacterId = id, Level = 1, Slot = slot });
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
            var teamMember = user.TeamMembers.Single(o => o.CharacterId == id);
            var worth = teamMember.Worth;

            if (user.TeamMembers.Count() > 1)
            {
                user.Gold += worth;
                db.TeamMembers.Remove(teamMember);

                if (ModelState.IsValid)
                {
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
    }
}