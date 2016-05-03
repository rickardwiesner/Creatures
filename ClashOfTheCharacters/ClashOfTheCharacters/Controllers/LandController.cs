using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Models;
using ClashOfTheCharacters.Services;
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
    public class LandController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        WildBattleService wildBattleService = new WildBattleService();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            if (!db.CurrentLands.Any(cl => cl.UserId == userId))
            {
                return RedirectToAction("Index", "Map");
            }

            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            return View(currentLand);
        }

        public ActionResult Battle()
        {
            var userId = User.Identity.GetUserId();

            if (!db.CurrentLands.Any(cl => cl.UserId == userId))
            {
                return RedirectToAction("Index", "Map");
            }

            if (!db.WildBattles.Any(wb => wb.UserId == userId))
            {
                wildBattleService.InitializeBattle(userId);
            }

            return View(db.WildBattles.First(wb => wb.UserId == userId));
        }

        [HttpPost]
        public ActionResult Attack()
        {
            var userId = User.Identity.GetUserId();
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);

            wildBattleService.Attack(userId);

            return RedirectToAction("Battle");
        }

        [HttpPost]
        public ActionResult Capture()
        {
            var userId = User.Identity.GetUserId();
            var wildBattle = db.WildBattles.First(wb => wb.UserId == userId);



            return RedirectToAction("Battle");
        }
    }
}