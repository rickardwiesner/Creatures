using ClashOfTheCharacters.Models;
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

        public ActionResult Land()
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

            var currentLand = db.CurrentLands.First(cl => cl.UserId == userId);

            var instance = new Random();
            var random = instance.Next(101);

            if (random == 1)
            {

            }


            //Kolla om motståndaren är på rätt stage
            //Slumpa fram en motståndare
            //Lägg till rätt level på gubbarna
            //Generera en battle

            return View();
        }
    }
}