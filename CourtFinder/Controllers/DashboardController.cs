using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CourtFinder.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.EntityFramework;
using System.IO;
using System.Globalization;

namespace CourtFinder.Controllers
{
    [RequireHttps]
    [Authorize]
    public class DashboardController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public ActionResult UserProfile()
        {
            string userid = User.Identity.GetUserId();
            ProfileViewModel model = new ProfileViewModel();
            Player player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            List<int> teamids = db.Teams.Where(val => val.Players.Select(p => p.PlayerID).Contains(player.PlayerID)).Select(val => val.TeamID).ToList();
            List<Team> teams = db.Teams.Where(val => teamids.Contains(val.TeamID)).ToList();
            model.player = player;
            model.teams = teams;
            return View(model);
        }

        [HttpPost]
        public ActionResult SaveProfile(ProfileViewModel model, string month, string day, string year, 
            string feetHeight, string inchesHeight, string lbWeight, string gender)
        {
            string userid = User.Identity.GetUserId();
            Player player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            player.FirstName = model.player.FirstName;
            player.LastName = model.player.LastName;
            player.BirthDate = DateTime.ParseExact(month + "/" + day + "/" + year, "MMM/dd/yyyy", CultureInfo.InvariantCulture);
            player.Height = (int.Parse(feetHeight) * 12) + int.Parse(inchesHeight);
            player.Weight = int.Parse(lbWeight);
            player.Gender = gender;
            db.SaveChanges();
            return View("UserProfile", model);
        }

        [HttpPost]
        public ActionResult CreateTeam(ProfileViewModel model)
        {
            string userid = User.Identity.GetUserId();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            string privateid = RandomString(5);
            while (db.Teams.Any(val => val.PrivateTeamID == privateid))
            {
                privateid = RandomString(5);
            }

            Team team = new Team { TeamName = model.newTeamName, Standing = 1.00, Wins = 0, Losses = 0, PrivateTeamID = privateid };
            team.Players.Add(me);
            db.Teams.Add(team);
            db.SaveChanges();

            return View("UserProfile", model);
        }

        [HttpPost]
        public ActionResult JoinTeam(ProfileViewModel model)
        {
            string userid = User.Identity.GetUserId();
            Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            Team team = db.Teams.Where(val => val.PrivateTeamID == model.joinTeamID).FirstOrDefault();
            team.Players.Add(me);
            db.SaveChanges();
            return View("UserProfile", model);
        }

        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/profile"), pic);
                file.SaveAs(path);
                string userid = User.Identity.GetUserId();
                Player me = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
                me.ProfileImage = "/images/profile/" + pic;
                db.SaveChanges();

            }
            return RedirectToAction("UserProfile", "Dashboard");
        }        

        [HttpGet]
        public ActionResult Team(string teamID)
        {
            Team team = db.Teams.Where(val => val.TeamID == int.Parse(teamID)).FirstOrDefault();
            TeamViewModel model = new TeamViewModel();
            model.team = team;
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateTeamName(TeamViewModel model)
        {
            Team team = db.Teams.Where(val => val.TeamID == model.team.TeamID).FirstOrDefault();
            team.TeamName = model.team.TeamName;
            db.SaveChanges();
            return View(model);
        }

        public ActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model)
        {
            return View();
        }  

        public ActionResult Facility()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Facility(FacilityViewModel model)
        {
            return View(model);
        }
    }
}