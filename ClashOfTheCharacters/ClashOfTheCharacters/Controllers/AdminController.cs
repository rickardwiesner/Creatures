using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClashOfTheCharacters.Models;
using Microsoft.AspNet.Identity;

namespace ClashOfTheCharacters.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId(); 
            var user = db.Users.Where(x => x.Id == userId);
            var isAdmin = false;

            foreach (var properties in user)
            {
                isAdmin = properties.Admin;
            }

            if (isAdmin == true)
            {
                return View(db.Creatures.ToList());
            }

            return RedirectToAction("Index", "Home");

        }

        // GET: Admin/Details/5
        public ActionResult Details(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == userId);
            var isAdmin = false;

            foreach (var properties in user)
            {
                isAdmin = properties.Admin;
            }
            if (isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Creature character = db.Creatures.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // GET: Admin/Create
        public ActionResult Create()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == userId);
            var isAdmin = false;

            foreach (var properties in user)
            {
                isAdmin = properties.Admin;
            }
            if (isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Element,ImageUrl,Price,BaseAttack,BaseDefense,BaseHp,AttackMultiplier,DefenseMultiplier,HpMultiplier")] Creature character)
        {
            if (ModelState.IsValid)
            {
                db.Creatures.Add(character);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(character);
        }

        // GET: Admin/Edit/5
        public ActionResult Edit(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == userId);
            var isAdmin = false;

            foreach (var properties in user)
            {
                isAdmin = properties.Admin;
            }

            if (isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Creature character = db.Creatures.Find(id);
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Element,ImageUrl,Price,BaseAttack,BaseDefense,BaseHp,AttackMultiplier,DefenseMultiplier,HpMultiplier")] Creature character)
        {
            if (ModelState.IsValid)
            {
                db.Entry(character).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(character);
        }

        // GET: Admin/Delete/5
        public ActionResult Delete(int? id)
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Where(x => x.Id == userId);
            var isAdmin = false;

            foreach (var properties in user)
            {
                isAdmin = properties.Admin;
            }

            if (isAdmin == false)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Creature character = db.Creatures.Find(id);
           
            if (character == null)
            {
                return HttpNotFound();
            }
            return View(character);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Creature character = db.Creatures.Find(id);
            db.Creatures.Remove(character);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
