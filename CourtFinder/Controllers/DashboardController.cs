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

        //this is used for creating a random teamid
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //contact google maps rest api and send address, bring into xml document, 
        //pull the lat and long coords out from the document
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
        }

        [HttpGet]
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

        [HttpPost]
        public ActionResult CreateLeague(FacilityViewModel model, int facilityID)
        {
            Facility facility = db.Facility.Where(val => val.FacilityID == facilityID).FirstOrDefault();
            if (model.LeagueName != null && model.LeagueName != "")
            {
                League league = new League
                {
                    LeagueName = model.LeagueName,
                    MaxTeams = model.maxTeams.GetValueOrDefault(),
                    MinTeams = model.minTeams.GetValueOrDefault(),
                    TeamSize = model.teamSize.GetValueOrDefault(),
                    Sport = db.Sports.Where(val => val.Description == model.sport).FirstOrDefault(),
                    RegisterStartPeriod = DateTime.Parse(model.registerStartMonth + "/" + model.registerStartDay + "/" + model.registerStartYear),
                    RegisterEndPeriod = DateTime.Parse(model.registerEndMonth + "/" + model.registerEndDay + "/" + model.registerEndYear)
                };
                facility.Leagues.Add(league);
                db.SaveChanges();
            }

            model.facility = facility;
            if (model.facility.Address != null)
            {
                model.address = model.facility.Address.Split(',')[0];
                model.state = model.facility.Address.Split(',')[1];
                model.zipCode = model.facility.Address.Split(',')[2];
            }
            model.facilitySports = model.facility.Sports.Select(val => val.Description).ToList();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            return View("Facility", model);
        }

        [HttpPost]
        public ActionResult AddCourt(FacilityViewModel model, string facilityID)
        {
            int intFacilityID = int.Parse(facilityID);
            Facility facility = db.Facility.Where(val => val.FacilityID == intFacilityID).FirstOrDefault();
            List<Sport> sports = db.Sports.Where(val => model.courtSports.Contains(val.Description)).ToList();

            Court court = new Court {
                CourtName = model.court.CourtName,
                Facility = facility,
                Sports = sports
            };
            db.Courts.Add(court);
            db.SaveChanges();

            model.facility = facility;
            if (model.facility.Address != null)
            {
                model.address = model.facility.Address.Split(',')[0];
                model.state = model.facility.Address.Split(',')[1];
                model.zipCode = model.facility.Address.Split(',')[2];
            }
            model.facilitySports = model.facility.Sports.Select(val => val.Description).ToList();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            return View("Facility", model);
        }

        [HttpGet]
        public ActionResult League(int leagueID)
        {
            string userID = User.Identity.GetUserId();
            LeagueViewModel model = new LeagueViewModel();
            model.league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            model.facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(model.league.LeagueID)).FirstOrDefault();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userID).FirstOrDefault();
            if (me != null)
            {
                model.myTeams = me.Teams.ToList();
            }
            return View(model);
        }

        //if it is register end period and min teams have signed up
        //auto create bracket (build games and schedule on courts)
        //based on scheduling alogrithm
        [HttpPost]
        public ActionResult League(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            //check if it is league settings and update those form fields
            if (model.league.LeagueName != null && model.league.LeagueName != "")
            {
                league.LeagueName = model.league.LeagueName;
                league.MaxTeams = model.maxTeams.GetValueOrDefault();
                league.MinTeams = model.minTeams.GetValueOrDefault();
                league.TeamSize = model.teamSize.GetValueOrDefault();
                league.Sport = db.Sports.Where(val => val.Description == model.sport).FirstOrDefault();
                league.RegisterStartPeriod = DateTime.Parse(model.registerStartMonth + "/" + model.registerStartDay + "/" + model.registerStartYear);
                league.RegisterEndPeriod = DateTime.Parse(model.registerEndMonth + "/" + model.registerEndDay + "/" + model.registerEndYear);
                db.SaveChanges();
            }

            model.league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            model.facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(model.league.LeagueID)).FirstOrDefault();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userID).FirstOrDefault();
            if (me != null)
            {
                model.myTeams = me.Teams.ToList();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult TeamSignup(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            Team team = db.Teams.Where(val => val.TeamID == model.teamSignupID).FirstOrDefault();
            //if we dont go over max teams, 
            //if it is during the sign up process, 
            //if our teamsize fits league setting,
            //if none of our team members are on another team in the league
            //the last check might fail because it wants us to compare ids not objects
            if (league.MaxTeams >= (league.Teams.Count + 1) &&
                (league.RegisterStartPeriod < DateTime.Now && league.RegisterEndPeriod > DateTime.Now) &&
                (league.TeamSize == team.Players.Count) &&
                (league.Teams.Where(t => team.Players.Any(tp => t.Players.Contains(tp))).Count() == 0))
            {
                league.Teams.Add(team);
                db.SaveChanges();
            }

            model.league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            model.facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(model.league.LeagueID)).FirstOrDefault();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userID).FirstOrDefault();
            if (me != null)
            {
                model.myTeams = me.Teams.ToList();
            }
            return View("League", model);
        }

        //abstract scheduling algorithm and append to when the gamewinner gets
        //set on the game page
        [HttpPost]
        public ActionResult CreateBracket(LeagueViewModel model, int leagueID)
        {
            string userID = User.Identity.GetUserId();
            League league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            if (model.gameStartMonth != 0)
            {
                //if create bracket, check number of teams fits min && max
                if (league.Teams.Count <= league.MaxTeams && league.Teams.Count >= league.MinTeams)
                {
                    List<Day> days = db.Days.Where(val => model.daysOfWeek.Contains(val.Description)).ToList();
                    List<Time> times = new List<Time>();// db.Times.ToList();
                    foreach (string selectedHour in model.hoursOfDay)
                    {
                        times.AddRange(db.Times.Where(val => val.Description.Hours == DateTime.Parse(selectedHour).TimeOfDay.Hours).ToList());
                    }
                    Bracket bracket = new Bracket
                    {
                        BracketStartDate = DateTime.Parse(model.gameStartMonth + "/" + model.gameStartDay + "/" + model.gameStartYear),
                        GameLength = new TimeSpan(model.gameLengthHour.GetValueOrDefault(), model.gameLengthMin.GetValueOrDefault(), 0),
                        Days = days,
                        Times = times //Available times //list of hours, 
                    };

                    //take all registered teams put into list, while list is not empty pull best vs worst
                    //create game based on timing specified in form and court availability
                    List<Team> unscheduledTeams = league.Teams.OrderBy(val => (val.Wins / val.Losses)).ToList();
                    List<Court> availableCourts = db.Courts.Where(val => val.Sports.Select(s => s.Description).Contains(league.Sport.Description)).ToList();
                    Team best, worst;
                    Game game;
                    TimeSpan gameLength = bracket.GameLength;
                    while (unscheduledTeams.Count != 0)
                    {
                        //uneven team count, list top team as bye week
                        if (unscheduledTeams.Count % 2 != 0)
                        {
                            bracket.UnscheduledTeams.Add(unscheduledTeams[0]);
                            unscheduledTeams.RemoveAt(0); 
                        }

                        //schedule best vs worse on available facility court, 
                        //and during bracket available days and times starting at bracketstartdate
                        best = unscheduledTeams[0];
                        worst = unscheduledTeams[unscheduledTeams.Count - 1];

                        //loop on available days and available times, start at bracketstartdate
                        //loop on court that support league sport
                        //move by gamelength and check if court is available
                        //if so set available court, available start
                        for (DateTime date = bracket.BracketStartDate; date <= (bracket.BracketStartDate.AddDays(model.daysBetweenRounds.GetValueOrDefault())); date.AddDays(1))
                        {
                            if (bracket.Days.Any(val => val.Description == date.DayOfWeek.ToString())) //if this day is a selected bracket day
                            {
                                //loop on times in selected bracket times
                                foreach (TimeSpan time in bracket.Times.Select(val => val.Description))
                                {
                                    DateTime currDate = date.Add(time);
                                    foreach (Court court in availableCourts)
                                    {
                                        //check if currdate to currDate + gametime has no scheduled game, if so grab time and court
                                        //and schedule game
                                        bool courtOverlap = court.Games.Any(val =>
                                            (currDate.Add(bracket.GameLength) > val.GameStart) && currDate < val.GameEnd);
                                        if (!courtOverlap)
                                        {
                                            game = new Game
                                            {
                                                Court = court, //available court
                                                GameStart = currDate, //available day and time
                                                GameEnd = currDate.Add(gameLength) //GameStart + game length
                                            };
                                            game.Teams.Add(best);
                                            game.Teams.Add(worst);
                                            bracket.Games.Add(game);
                                            unscheduledTeams.RemoveAt(0);
                                            unscheduledTeams.RemoveAt(unscheduledTeams.Count - 1);
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    league.Bracket = bracket;
                    db.SaveChanges();
                }           
            }
            model.league = db.Leagues.Where(val => val.LeagueID == leagueID).FirstOrDefault();
            model.facility = db.Facility.Where(val => val.Leagues.Select(l => l.LeagueID).Contains(model.league.LeagueID)).FirstOrDefault();
            model.sports = db.Sports.Select(val => val.Description).ToList();
            Player me = db.Players.Where(val => val.UserID == userID).FirstOrDefault();
            if (me != null)
            {
                model.myTeams = me.Teams.ToList();
            }
            return View("League", model);
        }

        [HttpGet]
        public ActionResult Game(string gameID)
        {
            GameViewModel model = new GameViewModel();
            int intGameID = int.Parse(gameID);
            model.game = db.Games.Where(val => val.GameID == intGameID).FirstOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult Game(GameViewModel model)
        {
            //here we need to check list of unscheduled teams and schedule a new game
            //for each team. Dont allow same teams to play eachother check previous games in bracket
            //schedule next game daysbetweenrounds after this game. Check if ever team has played all other teams
            //then we are done. if not done but cant schedule games yet, just add teams to unscheduled list
            //gonna use bracket results here

            int intGameID = 0;
            Team winningTeam = db.Teams.Where(val => val.TeamID == model.winningTeamID).FirstOrDefault();
            Game game = db.Games.Where(val => val.GameID == intGameID).FirstOrDefault();
            game.WinningTeam = winningTeam;
            db.SaveChanges();

            model.game = game;
            model.league = db.Leagues.Where(val => val.Bracket.Games.Select(g => g.GameID).Contains(game.GameID)).FirstOrDefault();
            return View(model);
        }

        [HttpGet]
        public ActionResult Court(string courtID)
        {
            CourtViewModel model = new CourtViewModel();
            int intCourtID = int.Parse(courtID);
            model.court = db.Courts.Where(val => val.CourtID == intCourtID).FirstOrDefault();
            return View(model);
        }

    }
}