using ClashOfTheCharacters.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    [Authorize]
    public class CreatureController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            if (db.UserCreatures.Any(tm => tm.UserId == userId))
            {
                var userCreatures = db.UserCreatures.Where(tm => tm.UserId == userId);
                return View(userCreatures.ToList());
            }

            return RedirectToAction("Select");
        }

        public ActionResult Select()
        {
            var userId = User.Identity.GetUserId();

            if (db.UserCreatures.Any(tm => tm.UserId == userId))
            {
                return RedirectToAction("Index");
            }

            var creatures = db.Creatures.Where(c => c.Name == "Aidan" || c.Name == "Bogey" || c.Name == "Mossy" || c.Name == "Horsish");

            return View(creatures.ToList());
        }

        [HttpPost]
        [ActionName("Select")]
        public ActionResult Select_Post()
        {
            var userId = User.Identity.GetUserId();
            int creatureId = Convert.ToInt32(Request.Form.Get("creatureId"));

            if (db.UserCreatures.Any(tm => tm.UserId == userId))
            {
                return RedirectToAction("Index");
            }

            db.UserCreatures.Add(new UserCreature { UserId = userId, CreatureId = creatureId, Level = 5, InSquad = true, Slot = 1 });
            db.SaveChanges();

            return RedirectToAction("Index", "Battle");
        }
    }
}