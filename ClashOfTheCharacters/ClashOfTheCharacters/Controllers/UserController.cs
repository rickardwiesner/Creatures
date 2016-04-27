using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet;
using Microsoft.AspNet.Identity;
using ClashOfTheCharacters.ViewModels;

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
                WonBattles = db.Competitors.Where(c => c.UserId == userId && c.Winner).Count() * 16,
                LostBattles = db.Competitors.Where(c => c.UserId == userId && !c.Winner).Count() * 11,
                MostUsedCharacter = user.TeamMembers.Any(tm => tm.BattleAppearances.Any()) ? user.TeamMembers.OrderByDescending(t => t.BattleAppearances.Count()).First() : null,
                TotalGoldEarned = db.Competitors.Any(c => c.UserId == userId) ? db.Competitors.Where(c => c.UserId == userId).Sum(c => c.GoldEarned) : 0
            };

            return View(userViewModel);
        }
    }
}