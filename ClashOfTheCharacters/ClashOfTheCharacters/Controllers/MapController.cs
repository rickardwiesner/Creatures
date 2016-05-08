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
    public class MapController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Map
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var travel = db.Travels.FirstOrDefault(t => t.UserId == userId);
            var currentLand = db.CurrentLands.FirstOrDefault(cl => cl.UserId == userId);
            var mapViewModel = new MapViewModel { Travel = travel, CurrentLand = currentLand };
            
            return View(mapViewModel);
        }

        public ActionResult Travel(int id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var land = db.Lands.Find(id);

            if (db.Travels.Any(t => t.UserId == userId))
            {
                return RedirectToAction("Travelling");
            }

            if (db.CurrentLands.Any(t => t.UserId == userId))
            {
                return RedirectToAction("Index", "Land");
            }

            if ((land.Element == Element.Darkness || land.Element == Element.Pollution || land.Element == Element.Nature || land.Element == Element.Light || land.Element == Element.Gravity) && !user.UnlockedLands.Any(ul => ul.LandId == id) || user.Stamina < land.Cost)
            {
                return RedirectToAction("Index");
            }

            return View(land);
        }

        [HttpPost]
        public ActionResult Travel()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var landId = Convert.ToInt32(Request.Form.Get("landId"));
            var land = db.Lands.Find(landId);

            if (user.Stamina < land.Cost)
            {
                return RedirectToAction("Index");
            }

            if (user.Stamina == user.MaxStamina)
            {
                user.LastStaminaTime = DateTimeOffset.Now;
            }

            user.Stamina -= land.Cost;

            var travel = new Travel { UserId = userId, LandId = landId, ArrivalTime = DateTimeOffset.Now.AddHours(land.Hours) };
            db.Travels.Add(travel);
            db.SaveChanges();

            return RedirectToAction("Travelling");
        }

        public ActionResult Travelling()
        {
            var userId = User.Identity.GetUserId();

            var travelService = new TravelService();
            travelService.CheckArrivalTime(userId);

            if (db.CurrentLands.Any(cl => cl.UserId == userId))
            {
                return RedirectToAction("Index", "Land");
            }

            if (!db.Travels.Any(t => t.UserId == userId))
            {
                return RedirectToAction("Index");
            }


            var travel = db.Travels.First(t => t.UserId == userId);
            return View(travel);
        }

        [HttpPost]
        [ActionName("Travelling")]
        public ActionResult Travelling_Post()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);

            if (!db.Travels.Any(t => t.UserId == userId))
            {
                return RedirectToAction("Index");
            }

            var travel = db.Travels.First(t => t.UserId == userId);

            if (user.Stamina + (travel.Land.Cost / 2) > user.MaxStamina)
            {
                user.Stamina = user.MaxStamina;
            }

            else
            {
                user.Stamina += (travel.Land.Cost / 2);
            }

            db.Travels.Remove(travel);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult FastTravel()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var rainbowGemCost = Convert.ToInt32(Request.Form.Get("rainbowGemCost"));

            if (!db.Travels.Any(t => t.UserId == userId) || user.RainbowGems < rainbowGemCost)
            {
                return RedirectToAction("Index");
            }

            user.RainbowGems -= rainbowGemCost;

            var travel = db.Travels.First(t => t.UserId == userId);
            travel.ArrivalTime = DateTimeOffset.Now;

            db.SaveChanges();

            var travelService = new TravelService();
            travelService.CheckArrivalTime(userId);

            return RedirectToAction("Index", "Land");
        }

        public ActionResult Shop()
        {
            return View(db.Items.ToList());
        }

        [HttpPost]
        public ActionResult Buy()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var itemId = Convert.ToInt32(Request.Form.Get("itemId"));
            var itemPrice = Convert.ToInt32(Request.Form.Get("itemPrice"));

            if (user.Gold >= itemPrice)
            {
                if (user.UserItems.Any(ui => ui.ItemId == itemId))
                {
                    user.UserItems.First(ui => ui.ItemId == itemId).Quantity++;
                }

                else
                {
                    db.UserItems.Add(new UserItem { ItemId = itemId, UserId = userId, Quantity = 1, InBag = true });
                }

                user.Gold -= itemPrice;

                db.SaveChanges();
            }

            return RedirectToAction("Shop");
        }
    }
}