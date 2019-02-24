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

        public ActionResult UserProfile(string playerID)
        {
            ProfileViewModel model = new ProfileViewModel();
            Player player;
            if (playerID == null || playerID == "")
            {
                string userid = User.Identity.GetUserId();
                player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();               
            }
            else
            {
                int intPlayerID = int.Parse(playerID);
                player = db.Players.Where(val => val.PlayerID == intPlayerID).FirstOrDefault();
            }
            
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
            if (model.profilePhoto != null)
            {
                string pic = System.IO.Path.GetFileName(model.profilePhoto.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/profile"), pic);
                model.profilePhoto.SaveAs(path);
                player.ProfileImage = "/images/profile/" + pic;
            }
            player.FirstName = model.player.FirstName;
            player.LastName = model.player.LastName;
            if (month != "" && day != "" && year != "")
                player.BirthDate = DateTime.Parse(month + "/" + day + "/" + year);
            if (feetHeight != "" && inchesHeight != "")
                player.Height = (int.Parse(feetHeight) * 12) + int.Parse(inchesHeight);
            if (lbWeight != "")
                player.Weight = int.Parse(lbWeight);
            if (gender != "")
                player.Gender = gender;
            if (player.Gender == "Male" && player.ProfileImage == "/Graphics/default_avatar.png")
                player.ProfileImage = "/Graphics/male_avatar.png";
            else if (player.Gender == "Female" && player.ProfileImage == "/Graphics/default_avatar.png")
                player.ProfileImage = "/Graphics/female_avatar.png";
            db.SaveChanges();
            return RedirectToAction("UserProfile", "Dashboard");
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

            model.player = me;
            model.teams = me.Teams.ToList();
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
            model.player = me;
            model.teams = me.Teams.ToList();
            return View("UserProfile", model);
        }       

        [HttpGet]
        public ActionResult Team(string teamID)
        {
            TeamViewModel model = new TeamViewModel();
            if (teamID != null)
            {
                int intTeamID = int.Parse(teamID);
                Team team = db.Teams.Where(val => val.TeamID == intTeamID).FirstOrDefault();
                
                model.team = team;
            }
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

        [HttpGet]
        public ActionResult Game(string gameID)
        {
            return View();
        }

    }
}