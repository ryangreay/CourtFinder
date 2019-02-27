using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CourtFinder.Models;

namespace CourtFinder.Controllers
{
    public class LeaguesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Leagues
        public ActionResult Index()
        {
            var leagues = db.Leagues.Include(l => l.Bracket);
            return View(leagues.ToList());
        }

        // GET: Leagues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Leagues/Create
        public ActionResult Create()
        {
            ViewBag.Sports = new SelectList(db.Sports, "Description", "Description");
            ViewBag.BracketID = new SelectList(db.Brackets, "BracketID", "BracketID");
            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LeagueID,RegisterStartPeriod,RegisterEndPeriod,TeamSize,EnrolledTeams,MinTeams,MaxTeams,BracketID,Sport")] League league)
        {
            league.EnrolledTeams = 0;
            if (ModelState.IsValid)
            {
                db.Leagues.Add(league);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Sports = new SelectList(db.Sports, "Description", "Description", league.Sport);
            ViewBag.BracketID = new SelectList(db.Brackets, "BracketID", "BracketID", league.BracketID);
            return View(league);
        }

        // GET: Leagues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            ViewBag.BracketID = new SelectList(db.Brackets, "BracketID", "BracketID", league.BracketID);
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "LeagueID,RegisterStartPeriod,RegisterEndPeriod,TeamSize,EnrolledTeams,MinTeams,MaxTeams,BracketID")] League league)
        {
            if (ModelState.IsValid)
            {
                db.Entry(league).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BracketID = new SelectList(db.Brackets, "BracketID", "BracketID", league.BracketID);
            return View(league);
        }

        // GET: Leagues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            League league = db.Leagues.Find(id);
            db.Leagues.Remove(league);
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
