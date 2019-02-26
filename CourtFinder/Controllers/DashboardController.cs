﻿using System;
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
using System.Device;
using System.Device.Location;
using System.Net;
using System.Configuration;
using System.Xml.Linq;

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

        public static GeoCoordinate GetCoordinates(string address)
        {
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&key=AIzaSyDccK65hnsCsuS5VmBDWaWo6BQoBwgcH78", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat").Value;
            var lng = locationElement.Element("lng").Value;
            return new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lng));
            //using (var client = new WebClient())
            //{
            //    string uri = "http://maps.google.com/maps/geo?q='" + region +
            //      "'&output=csv&key=" + ConfigurationManager.AppSettings["GoogleMapAPI"];
            //    string[] geocodeInfo = client.DownloadString(uri).Split(',');
            //    return new GeoCoordinate(Convert.ToDouble(geocodeInfo[2]), Convert.ToDouble(geocodeInfo[3]));
            //}
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
            model.team = team;
            return View(model);
        }

        [HttpGet]
        public ActionResult Search()
        {
            string userid = User.Identity.GetUserId();
            Player player = db.Players.Where(val => val.UserID == userid).FirstOrDefault();
            FacilityManager manager = db.FacilityManagers.Where(val => val.UserID == userid).FirstOrDefault();
            SearchViewModel model = new SearchViewModel();
            //get all facilities within default 10 mile radius from my location
            model.sports = db.Sports.ToList();
            model.Distance = 10;
            model.sport = null;
            List<Facility> facilities = new List<Facility>();
            if ((player != null && player.Latitude != 0) || (manager != null && manager.Latitude != 0))
            {
                GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(player != null ? player.Latitude : manager.Latitude),
                                                        Convert.ToDouble(player != null ? player.Longitude : manager.Longitude));
                facilities = db.Facility.AsEnumerable()
                    .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < (model.Distance * 1609.344)).ToList();
            }

            model.facilities = facilities;
            return View(model);
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel model, string distance, string sport, string lat, string lon)
        {
            if (distance != null && distance != "")
                model.Distance = int.Parse(distance);
            else
                model.Distance = 10;

            if (sport != null && sport != "" && sport != "Sport")
                model.sport = db.Sports.Where(val => val.Description == sport).FirstOrDefault();

            model.sports = db.Sports.ToList();
            //get all facilities with distance mile radius and supporting sport.
            //if facility does not have lat long set then dont add it to the list
            List<Facility> facilities = new List<Facility>();
            if (lat != null && lat != "" && lon != null && lon != "")
            {
                GeoCoordinate coord = new GeoCoordinate(Convert.ToDouble(lat), Convert.ToDouble(lon));
                facilities = db.Facility.AsEnumerable()
                    .Where(val => new GeoCoordinate(val.Latitude, val.Longitude).GetDistanceTo(coord) < (model.Distance * 1609.344)).ToList();
                facilities = facilities.Where(val => 
                    (model.sport != null && val.Sports.Select(s => s.SportID).Contains(model.sport.SportID)) ||
                    (model.sport == null)).ToList();
            }

            model.facilities = facilities;

            return View(model);
        }  

        [HttpGet]
        public ActionResult Facility(string facilityID)
        {
            int intFacilityID = 0;
            string userid = User.Identity.GetUserId();
            FacilityViewModel model = new FacilityViewModel();
            if (facilityID != null && facilityID != "")
            {
                intFacilityID = int.Parse(facilityID);
                model.facility = db.Facility.Where(val => val.FacilityID == intFacilityID).FirstOrDefault();
            }
            else
            {
                model.facility = db.Facility.Where(val => val.FacilityManager.UserID == userid).FirstOrDefault();
            }

            if (model.facility.Address != null)
            {
                model.address = model.facility.Address.Split(',')[0];
                model.state = model.facility.Address.Split(',')[1];
                model.zipCode = model.facility.Address.Split(',')[2];
            }
            model.facilitySports = model.facility.Sports.Select(val => val.Description).ToList();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Facility(FacilityViewModel model, string state, string facilityID)
        {
            int intFacilityID = int.Parse(facilityID);
            Facility me = db.Facility.Where(val => val.FacilityID == intFacilityID).FirstOrDefault();
            if (model.address != "" && model.address != null &&
                model.zipCode != "" && model.zipCode != null &&
                state != "" && state != null)
            {
                string address = model.address + ", " + state + ", " + model.zipCode;
                GeoCoordinate coords = GetCoordinates(address);
                me.Latitude = coords.Latitude;
                me.Longitude = coords.Longitude;
                me.Address = address;
            }
            if (model.facilitySports != null)
            {
                me.Sports.Clear();
                me.Sports = db.Sports.Where(val => model.facilitySports.Contains(val.Description)).ToList();
            }
            me.FacilityName = model.facility.FacilityName;
            model.facility = me;
            model.sports = db.Sports.Select(val => val.Description).ToList();

            db.SaveChanges();
            return View(model);
        }

        [HttpGet]
        public ActionResult Game(string gameID)
        {
            return View();
        }

    }
}