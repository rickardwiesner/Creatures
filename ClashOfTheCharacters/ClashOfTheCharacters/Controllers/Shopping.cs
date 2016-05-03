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
            var characters = db.Characters.Where(x => !userCharacters.Contains(x.Id)).ToList();
            var sellShop = new SellShopView()
            {
                ShoppingItems = characters,
                SellItems = user.TeamMembers.Select(o => o.Character).ToList()
            };
            ViewBag.Gold = user.Gold;
            var chars = db.Characters.ToList();
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

            if (gold >= characterPrice && user.TeamMembers.Count < 7)
            {
                var teamMember = new TeamMember { ApplicationUserId = userId, CharacterId = id, Level = 1 };
                db.TeamMembers.Add(teamMember);
                user.Gold -= characterPrice;
                db.SaveChanges();
                db.Dispose();
            }
            else
            {

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