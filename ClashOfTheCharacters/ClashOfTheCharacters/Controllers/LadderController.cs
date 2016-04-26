using ClashOfTheCharacters.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    public class LadderController : Controller
    {
        ApplicationDbContext context = new ApplicationDbContext();
        // GET: Ladder
        public ActionResult Index()
        {
            //Om det finns någon sträng i sökbaren sök, annars visa alla användare,
            string userSearch = Request["SearchInput"] == null ? "" : Request["SearchInput"];

            return View(context.Users.Where(c => c.UserName.Contains(userSearch)).OrderByDescending(c => c.LadderPoints));
        }
    }
}