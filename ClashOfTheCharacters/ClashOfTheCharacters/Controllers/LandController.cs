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
            ViewBag.OnGoingBattle = db.WildBattles.Any(wb => wb.UserId == userId) ? true : false;

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
                var user = db.Users.Find(userId);

                if (user.Stamina < 10)
                {
                    return RedirectToAction("Index");
                }

                wildBattleService.InitializeBattle(userId);
            }

            return View(db.WildBattles.First(wb => wb.UserId == userId));
        }

        [HttpPost]
        public ActionResult Attack()
        {
            var userId = User.Identity.GetUserId();

            if (db.WildBattles.Any(wb => wb.UserId == userId && !wb.Finished))
            {
                wildBattleService.Attack(userId);
            }

            return RedirectToAction("Battle");
        }

        [HttpPost]
        public ActionResult Capture()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var userItemId = Convert.ToInt32(Request.Form.Get("userItemId"));

            if (db.WildBattles.Any(wb => wb.UserId == userId && !wb.Finished) && user.UserItems.Any(ui => ui.Id == userItemId))
            {
                wildBattleService.Capture(userId, userItemId);
            }

            return RedirectToAction("Battle");
        }

        [HttpPost]
        public ActionResult Finish()
        {
            var userId = User.Identity.GetUserId();

            if (db.WildBattles.Any(wb => wb.UserId == userId) && db.CurrentLands.Any(wb => wb.UserId == userId)) 
            {
                wildBattleService.FinishBattle(userId);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Leave()
        {
            var userId = User.Identity.GetUserId();
            var wildBattle = db.WildBattles.FirstOrDefault(wb => wb.UserId == userId);
            var currentLand = db.CurrentLands.FirstOrDefault(wb => wb.UserId == userId);

            if (wildBattle != null)
            {
                db.WildBattles.Remove(wildBattle);
            }

            if (currentLand != null)
            {
                db.CurrentLands.Remove(currentLand);
            }

            db.SaveChanges();

            return RedirectToAction("Index", "Map");
        }
    }
}