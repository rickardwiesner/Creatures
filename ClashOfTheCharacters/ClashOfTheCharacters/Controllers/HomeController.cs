using ClashOfTheCharacters.Models;
using ClashOfTheCharacters.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var homeViewModel = new HomeViewModel
            {
                Users = db.Users.ToList(),
                Characters = db.Creatures.ToList()
            };

            return View(homeViewModel);
        }
        public ActionResult Characters(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Creature charact = db.Creatures.Find(id);
            if (charact == null)
            {
                return HttpNotFound();
            }
            return View(charact);
        }
    }
}