using ClashOfTheCharacters.Helpers;
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

            if (db.TeamMembers.Any(tm => tm.ApplicationUserId == userId))
            {
                var teamMembers = db.TeamMembers.Where(tm => tm.ApplicationUserId == userId);
                return View(teamMembers.ToList());
            }

            return RedirectToAction("Select");
        }

        public ActionResult Select()
        {
            var userId = User.Identity.GetUserId();

            if (db.TeamMembers.Any(tm => tm.ApplicationUserId == userId))
            {
                return RedirectToAction("Index");
            }

            if (!db.Characters.Any())
            {
                db.Characters.Add(new Character
                {
                    Name = "Aidan",
                    Element = Element.Fire,
                    ImageUrl = "/Images/Characters/aidan.jpg",
                    Price = 200,
                    BaseAttack = 48,
                    BaseDefense = 33,
                    BaseHp = 22,
                    AttackMultiplier = 2.12f,
                    DefenseMultiplier = 2.04f,
                    HpMultiplier = 2.06f,
                });

                db.Characters.Add(new Character
                {
                    Name = "Bogey",
                    Element = Element.Air,
                    ImageUrl = "/Images/Characters/bogey.jpg",
                    Price = 200,
                    BaseAttack = 44,
                    BaseDefense = 36,
                    BaseHp = 23,
                    AttackMultiplier = 2.11f,
                    DefenseMultiplier = 2.05f,
                    HpMultiplier = 2.05f,
                });

                db.Characters.Add(new Character
                {
                    Name = "Mossy",
                    Element = Element.Earth,
                    ImageUrl = "/Images/Characters/mossy.jpg",
                    Price = 200,
                    BaseAttack = 38,
                    BaseDefense = 40,
                    BaseHp = 24,
                    AttackMultiplier = 2.08f,
                    DefenseMultiplier = 2.08f,
                    HpMultiplier = 2.07f,
                });

                db.Characters.Add(new Character
                {
                    Name = "Horsish",
                    Element = Element.Water,
                    ImageUrl = "/Images/Characters/horsich.jpg",
                    Price = 200,
                    BaseAttack = 40,
                    BaseDefense = 41,
                    BaseHp = 23,
                    AttackMultiplier = 2.09f,
                    DefenseMultiplier = 2.07f,
                    HpMultiplier = 2.06f,
                });

                db.SaveChanges();
            }

            var characters = db.Characters.Where(c => c.Name == "Aidan" || c.Name == "Bogey" || c.Name == "Mossy" || c.Name == "Horsish");

            return View(characters.ToList());
        }

        public ActionResult Selected(int id)
        {
            var userId = User.Identity.GetUserId();

            var teamMember = new TeamMember { ApplicationUserId = userId, CharacterId = id, Level = 5, Slot = 1 };
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();

            return RedirectToAction("Index", "Battle");
        }

        [HttpPost]
        [ActionName("Select")]
        public ActionResult Select_Post()
        {
            var userId = User.Identity.GetUserId();
            int characterId = Convert.ToInt32(Request.Form.Get("characterId"));

            var teamMember = new TeamMember { ApplicationUserId = userId, CharacterId = characterId, Level = 5, Slot = 1 };
            db.TeamMembers.Add(teamMember);
            db.SaveChanges();

            return RedirectToAction("Index", "Battle");
        }
    }
}