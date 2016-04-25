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
    public class CharacterController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();

            if(db.TeamMembers.Any(tm => tm.ApplicationUserId == userId))
            {
                var teamMembers = db.TeamMembers.Where(tm => tm.ApplicationUserId == userId);
                return View(teamMembers.ToList());
            }

            return RedirectToAction("Select");
        }

        public ActionResult Select()
        {
            var userId = User.Identity.GetUserId();

            if(db.TeamMembers.Any(tm => tm.ApplicationUserId == userId))
            {
                return RedirectToAction("Index");
            }

            var characters = db.Characters.Where(c => c.Name == "Aidan" || c.Name == "Bogey" || c.Name == "Mossy" || c.Name == "Horsish");

            return View(characters.ToList());
        }

        [HttpPost]
        [ActionName("Select")]
        public ActionResult Select_Post()
        {
            var userId = User.Identity.GetUserId();
            int characterId = Convert.ToInt32(Request.Form.Get("characterId"));

            var teamMember = new TeamMember { ApplicationUserId = userId, CharacterId = characterId, Level = 5 };
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();

            return RedirectToAction("Index", "Battle");
        }
    }
}