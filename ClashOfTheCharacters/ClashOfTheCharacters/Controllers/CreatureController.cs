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

            if (db.UserCreatures.Any(uc => uc.UserId == userId))
            {
                var userCreatures = db.UserCreatures.Where(uc => uc.UserId == userId && !uc.InAuction);

                ViewBag.Travelling = db.Travels.Any(t => t.UserId == userId) || db.CurrentLands.Any(cl => cl.UserId == userId) ? true : false;

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

        public ActionResult AddToSquad(int userCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            int slot = 0;

            if (user.UserCreatures.Any(uc => uc.Id == userCreatureId) && user.UserCreatures.Count(uc => uc.InSquad) < 6 && !db.Travels.Any(t => t.UserId == userId) && !db.CurrentLands.Any(cl => cl.UserId == userId))
            {
                var userCreature = user.UserCreatures.First(uc => uc.Id == userCreatureId);

                if (!user.UserCreatures.Any(uc => uc.CreatureId == userCreature.CreatureId && uc.InSquad))
                {
                    for (int i = 1; i <= 6; i++)
                    {
                        if (!user.UserCreatures.Where(uc => uc.InSquad).Any(uc => uc.Slot == i))
                        {
                            slot = i;
                            break;
                        }
                    }

                    userCreature.InSquad = true;
                    userCreature.Slot = slot;

                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        public ActionResult Sell(int userCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Count > 1 && user.UserCreatures.Any(uc => uc.Id == userCreatureId && !uc.InSquad && !uc.InAuction))
            {
                var userCreature = user.UserCreatures.First(uc => uc.Id == userCreatureId && !uc.InSquad && !uc.InAuction);
                return View(userCreature);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("Sell")]
        public ActionResult Sell_Post(int userCreatureId)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (user.UserCreatures.Count > 1 && user.UserCreatures.Any(uc => uc.Id == userCreatureId && !uc.InSquad && !uc.InAuction))
            {
                var userCreature = user.UserCreatures.First(uc => uc.Id == userCreatureId && !uc.InSquad && !uc.InAuction);
                user.Gold += userCreature.Worth;
                db.UserCreatures.Remove(userCreature);

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}