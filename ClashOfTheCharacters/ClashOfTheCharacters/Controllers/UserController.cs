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
                WonBattles = db.Competitors.Where(c => c.UserId == userId && c.Winner).Count(),
                LostBattles = db.Competitors.Where(c => c.UserId == userId && !c.Winner).Count(),
                TeamMember = user.TeamMembers.OrderByDescending(t => t.BattleAppearances.Count()).First(),
            };

            return View(userViewModel);
        }
    }
}