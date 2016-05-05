using ClashOfTheCharacters.Models;
using ClashOfTheCharacters.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClashOfTheCharacters.Controllers
{
    public class LadderController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var ladderViewModels = new List<LadderViewModel>();
            int rank = 0;

            foreach (var user in db.Users.OrderByDescending(u => u.LadderPoints).ToList())
            {
                rank++;
                ladderViewModels.Add(new LadderViewModel { User = user, Rank = rank });
            }

            //Om det finns någon sträng i sökbaren sök, annars visa alla användare,
            if (!string.IsNullOrEmpty(Request["SearchInput"]))
            {
                var userSearch = Request["SearchInput"];
                return View(ladderViewModels.Where(lvm => lvm.User.UserName.ToLower().Contains(userSearch.ToLower())).OrderByDescending(lvm => lvm.User.LadderPoints).ToList());
            }

            else
            {
                return View(ladderViewModels);
            }
        }
    }
}