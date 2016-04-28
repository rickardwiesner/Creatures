using ClashOfTheCharacters.Helpers;
using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCreature()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            string nameInput = Request["cNameInput"];
            //Fixa en dropdown i HTMLen för alla element man kan välja?
            //Element elementInput = (Element)Enum.Parse(typeof(Element), Request["elementInput"]);
            string imageUrlInput = Request["imageUrlInput"];
            int princeInput = Convert.ToInt32(Request["priceInput"]);
            int baseAttackInput = Convert.ToInt32(Request["baseAttackInput"]);
            int baseDefenseInput = Convert.ToInt32(Request["baseDefenseInput"]);
            int baseHpInput = Convert.ToInt32(Request["baseHpInput"]);
            float attMultiplierInput = float.Parse(Request["attMultiplierInput"], CultureInfo.InvariantCulture.NumberFormat);
            float defMultiplierInput = float.Parse(Request["defMultiplierInput"], CultureInfo.InvariantCulture.NumberFormat);
            float hpMultiplierInput = float.Parse(Request["attMultiplierInput"], CultureInfo.InvariantCulture.NumberFormat);


            var newCreature = new Character
            {
                Name = nameInput,
                Element = Element.Fire, //Hårdkodat atm läs kommentar ovan och i Admin/Index.Html
                ImageUrl = imageUrlInput,
                Price = princeInput,
                BaseAttack = baseAttackInput,
                BaseDefense = baseDefenseInput,
                BaseHp = baseHpInput,
                AttackMultiplier = attMultiplierInput,
                DefenseMultiplier = defMultiplierInput,
                HpMultiplier = hpMultiplierInput
            };

            context.Characters.Add(newCreature);
            context.SaveChanges();
            return Redirect("/Admin/Index");
        }
    }
}